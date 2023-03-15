using System;
using System.Collections.Generic;
using RemaSoftware.Domain.Models;
using RemaSoftware.WebApp.DTOs;

namespace RemaSoftware.WebApp.Hub;
using Microsoft.AspNetCore.SignalR;

public class ProductionHub : Hub
{
    private readonly IServiceProvider _services;

    public ProductionHub(IServiceProvider services)
    {
        _services = services;
    }
    public void Send(ProductionAnalysisDto paDtos)
    {
        try
        {
            var ctx = _services.GetService(typeof(IHubContext<ProductionHub>)) as IHubContext<ProductionHub>;
            ctx.Clients.All.SendAsync("broadcastMessage", paDtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}