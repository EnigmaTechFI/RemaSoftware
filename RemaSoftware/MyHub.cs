using System;
using Microsoft.AspNetCore.SignalR;

namespace RemaSoftware
{
    public class MyHub : Hub
    {
        private readonly IServiceProvider _services;

        public MyHub(IServiceProvider services)
        {
            _services = services;
        }
        public void Send(string name, string message)
        {
            var ctx = _services.GetService(typeof(IHubContext<MyHub>)) as IHubContext<MyHub>;
            ctx.Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}