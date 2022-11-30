using Microsoft.AspNetCore.Mvc.Rendering;
using RemaSoftware.Domain.Models;
using System.Collections.Generic;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class UpdateOrderViewModel
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

        public UpdateOrderViewModel()
        {
            this.OperationsSelected = new List<string>();
        }
    }
}
