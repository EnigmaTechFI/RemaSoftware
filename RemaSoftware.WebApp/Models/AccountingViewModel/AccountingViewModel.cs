using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.AccountingViewModel
{
    public class AccountingViewModel
    {
        public List<OrderInFactoryGroupByClient> OrdersInFactoryGroupByClient { get; set;}
        public List<Order> OrdersNotCompleted { get; set; }

        public class OrderInFactoryGroupByClient
        {
            public string Client { get; set; }
            public int NumberPiecesInStock { get; set; }
            public decimal TotPriceInStock { get; set; }
        }
    }
}
