using System.Collections.Generic;
using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.GuestViewModel;

namespace RemaSoftware.WebApp.Helper;

public class GuestHelper
{
    private readonly IOrderService _orderService;
    private readonly IClientService _clientService;
    private readonly ISubBatchService _subBatchService;

    public GuestHelper(IClientService clientService, IOrderService orderService, ISubBatchService subBatchService)
    {
        _clientService = clientService;
        _orderService = orderService;
        _subBatchService = subBatchService;
    }

    public IndexViewModel GetIndexViewModel(string userId)
    {
        var clientId = _clientService.GetClientIdByUserId(userId);
        var stock = _orderService.GetDdtInStockByClientId(clientId);
        var working = _orderService.GetDdtInWorkingByClientId(clientId);
        var totalpieces = stock.Sum(s => s.Number_Piece_Now) + working.Sum(s => s.Number_Piece_Now);
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
        var a = _orderService.GetDdtOutsByClientIdAndStatus(clientId, DDTOutStatus.STATUS_PENDING);
        return new DDTInDeliveryViewModel()
        {
            DdtOut = _orderService.GetDdtOutsByClientIdAndStatus(clientId, DDTOutStatus.STATUS_PENDING)
        };
    }
}