#SignalR.MongoRabbit

#About

SignalR.RabbitMq (https://github.com/mdevilliers/SignalR.RabbitMq) is an implementation of an ScaleOutMessageBus using RabbitMq as the backing store. 
This allows a signalr web application to be scaled across a web farm. This library builds on top of it to eliminate the need for the custom RabbitMQ plugin. 
It keeps track of message IDs in MongoDB.

#Installation

.Net
----

A compiled library is available via NuGet

To install via the nuget package console

```PS
Install-Package SignalR.RabbitMq
```

To install via the nuget user interface in Visual Studio the package to search for is "SignalR.RabbitMq"

#Usage

General Usage
-------------

The example web project shows how you could configure the message bus in the global.asax.cs file.

```CSHARP
var factory = new ConnectionFactory 
{ 
	UserName = "guest",
	Password = "guest"
};


var mongoConnectionString = "mongodb://localhost/signalr-test";
var exchangeName = "SignalR.RabbitMQ-Example";
var configuration = new MongoRabbitScaleoutConfiguration(mongoConnectionString, factory, exchangeName);
var connection = new MongoRabbitConnection(configuration);
GlobalHost.DependencyResolver.UseRabbitMqAdvanced(connection, configuration);

```

See also documentation at https://github.com/mdevilliers/SignalR.RabbitMq.