using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SignalR.MongoRabbit.Example.Hubs
{
    public class Chat : Hub
    {
        private const string GroupName = "SecretGroup";
        private static int _id;

        public void Send(string message)
        {
            Clients.All.addMessage(string.Format("{0} - {1}", message, _id), Context.ConnectionId);
            Clients.Others.addMessage(string.Format("Some one said - {0} - {1}", message, _id), Context.ConnectionId);
            _id++;
        }

        public void SendGroup(string message)
        {
            Clients.Group(GroupName).addMessage(string.Format("{0} - {1}", message, _id), GroupName);
            _id++;
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, GroupName);
        }
    }
}