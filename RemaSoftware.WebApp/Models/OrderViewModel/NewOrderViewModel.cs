using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RemaSoftware.WebApp.Models.Common;
using RemaSoftware.Domain.ContextModels;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class NewOrderViewModel
    {
        public List<Client> Clients { get; set; }
        public Order Order { get; set; }
        public string DataOutStr { get; set; }
        public decimal? uni_price { get; set; }
        public List<string> OldOrders_SKU { get; set; }
        public List<SelectListItem> Operations { get; set; }
        public string Photo { get; set; }
        public string RedirectUrlAfterCreation { get; set; }
        public List<string> OperationsSelected { get; set; }

        public NewOrderViewModel()
        {
            this.OperationsSelected = new List<string>();
        }
    }
}
