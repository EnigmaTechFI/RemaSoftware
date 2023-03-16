using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.GuestViewModel;

namespace RemaSoftware.WebApp.Helper;

public class GuestHelper
{
    private readonly IOrderService _orderService;
    private readonly IClientService _clientService;

    public GuestHelper(IClientService clientService, IOrderService orderService)
    {
        _clientService = clientService;
        _orderService = orderService;
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
}