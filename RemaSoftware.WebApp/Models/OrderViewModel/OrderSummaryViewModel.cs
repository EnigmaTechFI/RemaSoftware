using System.Collections.Generic;
using RemaSoftware.Domain.ContextModels;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class OrderSummaryViewModel
    {
        public List<Order> Orders { get; set; }
        public string RedirectUrlAfterCreation { get; set; }
    }
}
