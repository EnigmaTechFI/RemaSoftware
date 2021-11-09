using RemaSoftware.ContextModels;
using System.Collections.Generic;
using RemaSoftware.Models.Common;

namespace RemaSoftware.Models.OrderViewModel
{
    public class NewOrderViewModel
    {
        public List<Client> Clients { get; set; }
        public Order Order { get; set; }
        public string DataOutStr { get; set; }
        public decimal? uni_price { get; set; }
        public List<string> OldOrders_SKU { get; set; }
        public List<OperationFlag> Operations { get; set; }
        public string Photo { get; set; }
        public string RedirectUrlAfterCreation { get; set; }

    }
}
