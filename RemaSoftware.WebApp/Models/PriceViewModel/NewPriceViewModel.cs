using RemaSoftware.Domain.Models;
using System.Collections.Generic;

namespace RemaSoftware.WebApp.Models.PriceViewModel
{
    public class NewPriceViewModel
    {
        public Price Price { get; set; }
        public List<Product> Products { get; set; }
        public List<Operation> Operations { get; set; }
        public string PriceVal { get; set; }
        public string SelectedOperationIDs { get; set; }
    }
}
