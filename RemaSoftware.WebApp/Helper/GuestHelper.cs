using System;
using System.Collections.Generic;
using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Models.GuestViewModel;

namespace RemaSoftware.WebApp.Helper;

public class GuestHelper
{
    private readonly IOrderService _orderService;
    private readonly IClientService _clientService;
    private readonly ISubBatchService _subBatchService;
    private readonly IEmailService _emailService;
    public GuestHelper(IClientService clientService, IOrderService orderService, ISubBatchService subBatchService, IEmailService emailService)
    {
        _clientService = clientService;
        _orderService = orderService;
        _subBatchService = subBatchService;
        _emailService = emailService;
    }

    public IndexViewModel GetIndexViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        var stock = _orderService.GetDdtInStockByClientId(clientId);
        var working = _orderService.GetDdtInWorkingByClientId(clientId);
        var totalpieces = stock.Sum(s => s.Number_Piece_Now + s.Number_Piece_Now) + working.Sum(s => s.Number_Piece_Now + s.Number_Piece_ToSupplier);
        var ddtOuts = _orderService.GetDdtOutsByClientIdAndStatus(clientId, DDTOutStatus.STATUS_PENDING);
        var vm = new IndexViewModel()
        {
            DDTStock = stock.Count,
            DDTWorking = working.Count,
            NumberPiecesReady = ddtOuts.Count != 0 ? ddtOuts[0].Ddt_Associations.Sum(s =>s.NumberPieces) : 0,
            TotalPieces = totalpieces
        };
        return vm;
    }

    public OrdersActiveViewModel GetOrdersActiveViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        return new OrdersActiveViewModel()
        {
            Ddt_In = _orderService.GetDdtInActiveByClientId(clientId)
        };
    }

    public OrdersEndedViewModel GetOrdersEndedViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        return new OrdersEndedViewModel()
        {
            Ddt_In = _orderService.GetDdtInEndedByClientId(clientId)
        };
    }

    public BatchInViewModel GetBatchInStockViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        return new BatchInViewModel()
        {
            SubBatches = _subBatchService.GetSubBatchesStatusAndClientId(OrderStatusConstants.STATUS_ARRIVED, clientId)
        };
    }

    public BatchInViewModel GetBatchInProductionViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        var sb = new List<SubBatch>();
        sb.AddRange(_subBatchService.GetSubBatchesStatusAndClientId(OrderStatusConstants.STATUS_WORKING, clientId));
        sb.AddRange(_subBatchService.GetSubBatchesStatusAndClientId(OrderStatusConstants.STATUS_PARTIALLY_COMPLETED, clientId));
        return new BatchInViewModel()
        {
            SubBatches = sb
        };
    }

    public DDTInDeliveryViewModel GetBatchInDeliveryViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        return new DDTInDeliveryViewModel()
        {
            DdtOut = _orderService.GetDdtOutsByClientIdAndStatus(clientId, DDTOutStatus.STATUS_PENDING)
        };
    }

    public GuestSubBatchMonitoringViewModel GetSubBatchMonitoring(int id, string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        return new GuestSubBatchMonitoringViewModel()
        {
            SubBatch = _subBatchService.GetSubBatchByIdAndClientId(id, clientId)?? throw new Exception("Errore durante il recupero.")
        };
    }

    public void SendPrompt(int ddtId, IList<MyUser> users, string userId)
    {
        var ddt = _orderService.GetDdtInById(ddtId);
        if (ddt.Product.ClientID != _clientService.GetClientIdByUserId(userId))
            throw new Exception("Errore durante la richiesta di sollecito.");
        ddt.IsPrompted = true;
        _orderService.UpdateDDtIn(ddt);
        _emailService.SendEmailPrompt(users.Select(s => s.Email).ToList(), ddt.Code);
    }
}