using System.Collections.Generic;
using RemaSoftware.WebApp.ContextModels;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class OrderSummaryViewModel
    {
        public List<Order> Orders { get; set; }
        public string RedirectUrlAfterCreation { get; set; }
    }
}
