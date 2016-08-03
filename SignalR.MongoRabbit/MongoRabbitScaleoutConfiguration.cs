using System.Collections.Generic;
using RabbitMQ.Client;
using SignalR.RabbitMQ;

namespace SignalR.MongoRabbit
{
    public class MongoRabbitScaleoutConfiguration : RabbitMqScaleoutConfiguration
    {
        public MongoRabbitScaleoutConfiguration(string mongoConnectionString, ConnectionFactory connectionfactory,
            string exchangeName, string queueName = null)
            : base(connectionfactory, exchangeName, queueName, "signalr-stamp")
        {
            MongoConnectionString = mongoConnectionString;
        }

        public string MongoConnectionString { get; set; }

        public string CollectionName { get; set; } = "signalr.mongorabbit";

        public List<string> RabbitMqClusterMembers { get; set; }
    }
}