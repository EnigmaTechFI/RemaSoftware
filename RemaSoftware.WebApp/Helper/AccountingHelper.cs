using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using static RemaSoftware.WebApp.Models.AccountingViewModel.AccountingViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountingHelper
    {
        private readonly IOrderService _orderService;
        private readonly ISubBatchService _subBatchService;

        public AccountingHelper(IOrderService orderService, ISubBatchService subBatchService)
        {
            _orderService = orderService;
            _subBatchService = subBatchService;
        }

        public ProductionAnalysisLiveViewModel GetProductionAnalysisLiveViewModel()
        {
            return new ProductionAnalysisLiveViewModel()
            {
                OperationTimeLines = _subBatchService.GetOperationTimelinesByStatus(OperationTimelineConstant.STATUS_WORKING)
            };
        }
        public AccountingViewModel GetAccountingViewModel()
        {
            var ordersInFactory = _orderService.GetDdtInActive()
                .GroupBy(c => c.Product.ClientID)
                .Select(s =>
                    new OrderInFactoryGroupByClient
                    {
                        Client = s.First().Product.Client.Name,
                        NumberPiecesInStock = s.Sum(sum => sum.Number_Piece_Now),
                        TotPriceInStock = s.Sum(sum => sum.Price_Uni * sum.Number_Piece_Now)
                    }).ToList();
            var vm = new AccountingViewModel
            {
                OrdersInFactoryGroupByClient = ordersInFactory,
                OrdersNotCompleted = _orderService.GetDdtInActive().OrderBy(s => s.Product.Client.Name).ToList()
            };
            return vm;
        }
    }
}
