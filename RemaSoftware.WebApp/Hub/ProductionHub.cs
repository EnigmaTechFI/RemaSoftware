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
    
    public void StartOperation(ProductionAnalysisDto paDtos)
    {
        try
        {
            var ctx = _services.GetService(typeof(IHubContext<ProductionHub>)) as IHubContext<ProductionHub>;
            ctx.Clients.All.SendAsync("startOperation", paDtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void EndOperation(int operationTimelinId, int machineId)
    {
        try
        {
            var ctx = _services.GetService(typeof(IHubContext<ProductionHub>)) as IHubContext<ProductionHub>;
            ctx.Clients.All.SendAsync("endOperation", operationTimelinId, machineId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}