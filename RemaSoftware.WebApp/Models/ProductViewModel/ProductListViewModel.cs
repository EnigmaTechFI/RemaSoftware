using RemaSoftware.Domain.Models;
using System.Collections.Generic;

namespace RemaSoftware.WebApp.Models.ProductViewModel
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public SubBatch SubBatch { get; set; }
    }
}
