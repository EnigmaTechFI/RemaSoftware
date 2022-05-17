using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.WebApp.ContextModels;

namespace RemaSoftware.WebApp.Models.AccountingViewModel
{
    public class AccountingViewModel
    {
        public List<OrderInFactoryGroupByClient> orderInFactoryGroupByClient { get; set;}
        public List<Order> ordersNotCompleted { get; set; }

        public class OrderInFactoryGroupByClient
        {
            public string Client { get; set; }
            public int NumberPiecesInStock { get; set; }
            public decimal TotPriceInStock { get; set; }
        }
    }
}
