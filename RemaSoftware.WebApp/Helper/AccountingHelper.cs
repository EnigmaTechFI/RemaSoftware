using System;
using System.Linq;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using static RemaSoftware.WebApp.Models.AccountingViewModel.AccountingViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountingHelper
    {
        private readonly IOrderService _orderService;
        private readonly ISubBatchService _subBatchService;
        private readonly IEmailService _emailService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
        private readonly ApplicationDbContext _dbContext;

        public AccountingHelper(IOrderService orderService, ISubBatchService subBatchService, IEmailService emailService, ApplicationDbContext dbContext, IAPIFatturaInCloudService apiFatturaInCloudService)
        {
            _orderService = orderService;
            _subBatchService = subBatchService;
            _emailService = emailService;
            _dbContext = dbContext;
            _apiFatturaInCloudService = apiFatturaInCloudService;
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

        public DdtVariationViewModel GetDdtVariationViewModel()
        {
            return new DdtVariationViewModel()
            {
                SubBatches = _subBatchService.GetSubBatchesWithExtras()
            };
        }

        public void ConfirmVariation(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(id);
                    ddt.Price_Uni = ddt.PendingPrice;
                    ddt.PendingPrice = 0;
                    ddt.PriceIsPending = false;
                    _orderService.UpdateDDtIn(ddt);
                    _apiFatturaInCloudService.EditDdtInCloud(ddt, ddt.Product.SKU);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public void DeleteVariation(int id)
        {
            var ddt = _orderService.GetDdtInById(id);
            ddt.PendingPrice = 0;
            ddt.PriceIsPending = false;
            _orderService.UpdateDDtIn(ddt);
        }

        public void RequestVariation(int id, decimal price, string mail, string message)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var ddt = _orderService.GetDdtInById(id);
                    ddt.PendingPrice = price;
                    ddt.PriceIsPending = true;
                    _orderService.UpdateDDtIn(ddt);
                    _emailService.SendEmailPriceVariation(price, mail, message, ddt.Code, ddt.Product.Client.Name);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
    }
}
