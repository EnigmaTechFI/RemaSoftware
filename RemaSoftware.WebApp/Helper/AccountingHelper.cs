using System.Collections.Generic;
using System.Linq;
using RemaSoftware.WebApp.DALServices;
using RemaSoftware.WebApp.Data;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using static RemaSoftware.WebApp.Models.AccountingViewModel.AccountingViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountingHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderService _orderService;

        public AccountingHelper(ApplicationDbContext dbContext, IOrderService orderService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
        }

        public List<AccountingViewModel.OrderInFactoryGroupByClient> GetDataForAccounting()
        {
            var accounting = _orderService.GetOrdersNotCompleted()
                .GroupBy(c => c.ClientID)
                .Select(s =>
                new AccountingViewModel.OrderInFactoryGroupByClient
                {
                    Client = s.First().Client.Name,
                    NumberPiecesInStock = s.Sum(sum => sum.Number_Pieces_InStock),
                    TotPriceInStock = s.Sum(sum => sum.Price_InStock)
                }).ToList(); ;
            return accounting;
        }
    }
}
