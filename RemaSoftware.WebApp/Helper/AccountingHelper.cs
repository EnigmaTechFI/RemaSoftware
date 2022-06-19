using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Data;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using static RemaSoftware.WebApp.Models.AccountingViewModel.AccountingViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountingHelper
    {
        private readonly IOrderService _orderService;

        public AccountingHelper(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public AccountingViewModel GetAccountingViewModel()
        {
            var ordersInFactory = _orderService.GetOrdersNotCompleted()
                .GroupBy(c => c.ClientID)
                .Select(s =>
                    new AccountingViewModel.OrderInFactoryGroupByClient
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
