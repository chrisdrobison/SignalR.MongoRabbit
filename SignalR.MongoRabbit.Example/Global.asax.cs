using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using RabbitMQ.Client;
using SignalR.RabbitMQ;

namespace SignalR.MongoRabbit.Example
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var factory = new ConnectionFactory
            {
                UserName = "vh2user",
                Password = "Password1",
                HostName = "xprmuttmq1",
                AutomaticRecoveryEnabled = true,
                VirtualHost = "vh2"
            };

            var mongoConnectionString = "mongodb://localhost/signalr-test";
            var exchangeName = "SignalR.RabbitMQ-Example";
            var configuration = new MongoRabbitScaleoutConfiguration(mongoConnectionString, factory, exchangeName);
            var connection = new MongoRabbitConnection(configuration);
            GlobalHost.DependencyResolver.UseRabbitMqAdvanced(connection, configuration);
        }
    }
}