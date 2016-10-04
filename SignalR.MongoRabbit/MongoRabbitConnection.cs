using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using SignalR.RabbitMQ;

namespace SignalR.MongoRabbit
{
    public class MongoRabbitConnection : RabbitConnectionBase
    {
        private const string MessageIdHeader = "signalr-message-id";

        private readonly MongoRabbitScaleoutConfiguration _configuration;
        private readonly IMongoCollection<MongoRabbitCount> _collection;
        private IConnection _rabbitConnection;
        private IModel _receiveModel;
        private IModel _publishModel;

        public MongoRabbitConnection(MongoRabbitScaleoutConfiguration configuration) : base(configuration)
        {
            if (string.IsNullOrEmpty(configuration.MongoConnectionString))
            {
                throw new ArgumentNullException(nameof(configuration.MongoConnectionString),
                    "Connection string cannot be null or empty");
            }

            if (string.IsNullOrEmpty(configuration.CollectionName))
            {
                throw new ArgumentNullException(nameof(configuration.CollectionName),
                    "Collection name cannot be null or empty");
            }

            _configuration = configuration;
            var mongoUrl = new MongoUrl(configuration.MongoConnectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _collection = database.GetCollection<MongoRabbitCount>(configuration.CollectionName);
        }

        public override void Send(RabbitMqMessageWrapper message)
        {
            var properties = new BasicProperties
            {
                Headers = new Dictionary<string, object>
                {
                    {MessageIdHeader, GetNewMessageId().ToString()}
                }
            };
            _publishModel.BasicPublish(Configuration.ExchangeName, string.Empty, properties, message.Bytes);
        }

        public override void StartListening()
        {
            _rabbitConnection = _configuration.RabbitMqClusterMembers?.Count > 0
                ? _configuration.ConnectionFactory.CreateConnection(_configuration.RabbitMqClusterMembers)
                : _configuration.ConnectionFactory.CreateConnection();
            _publishModel = _rabbitConnection.CreateModel();
            _receiveModel = _rabbitConnection.CreateModel();
            _receiveModel.ExchangeDeclare(_configuration.ExchangeName, ExchangeType.Fanout, true);
            var queue = _configuration.QueueName == null
                ? _receiveModel.QueueDeclare()
                : _receiveModel.QueueDeclare(_configuration.QueueName, true, false, false,
                    new Dictionary<string, object>());
            _receiveModel.QueueBind(queue.QueueName, _configuration.ExchangeName, "");

            var consumer = new EventingBasicConsumer(_receiveModel);
            consumer.Received += (sender, args) =>
            {
                var message = new RabbitMqMessageWrapper
                {
                    Bytes = args.Body,
                    Id =
                        Convert.ToUInt64(Encoding.UTF8.GetString((byte[]) args.BasicProperties.Headers[MessageIdHeader]))
                };
                OnMessage(message);
            };

            _receiveModel.BasicConsume(queue.QueueName, true, consumer);
        }

        public override void Dispose()
        {
            _publishModel.Dispose();
            _receiveModel.Dispose();
            _rabbitConnection.Dispose();

            base.Dispose();
        }

        private ulong GetNewMessageId()
        {
            var filter = new BsonDocument();
            var update = Builders<MongoRabbitCount>.Update.Inc(count => count.Count, (ulong) 1);
            var result = _collection.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<MongoRabbitCount, MongoRabbitCount>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                }).GetAwaiter().GetResult();
            return result.Count;
        }
    }
}