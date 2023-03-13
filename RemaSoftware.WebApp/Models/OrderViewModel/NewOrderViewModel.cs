using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class NewOrderViewModel
    {
        public List<Client> Clients { get; set; }
        public Ddt_In Ddt_In { get; set; }
        public List<SelectListItem> Operations { get; set; }
        public List<string> OperationsSelected { get; set; }
        public decimal? uni_price { get; set; }

        public NewOrderViewModel()
        {
            this.OperationsSelected = new List<string>();
        }
    }
}
