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
            var ordersInFactory = _orderService.GetOrdersNotCompleted()
                .GroupBy(c => c.ClientID)
                .Select(s =>
                    new OrderInFactoryGroupByClient
                    {
                        Client = s.First().Client.Name,
                        NumberPiecesInStock = s.Sum(sum => sum.Number_Pieces_InStock),
                        TotPriceInStock = s.Sum(sum => sum.Price_InStock)
                    }).ToList();
            var vm = new AccountingViewModel
            {
                OrdersInFactoryGroupByClient = ordersInFactory,
                OrdersNotCompleted = _orderService.GetOrdersNotCompleted().OrderBy(s => s.Client.Name).ToList()
            };
            return vm;
        }
    }
}
