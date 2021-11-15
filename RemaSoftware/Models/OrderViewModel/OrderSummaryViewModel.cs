using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Models.OrderViewModel
{
    public class OrderSummaryViewModel
    {
        public List<Order> Orders { get; set; }
        public string RedirectUrlAfterCreation { get; set; }
    }
}
