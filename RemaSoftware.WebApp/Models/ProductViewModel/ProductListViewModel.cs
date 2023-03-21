using RemaSoftware.Domain.Models;
using System.Collections.Generic;

namespace RemaSoftware.WebApp.Models.ProductViewModel
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public LabelToPrint LabelToPrint { get; set; }
    }

    public class LabelToPrint
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string ClientName { get; set; }
        public string OperationList { get; set; }
    }
}
