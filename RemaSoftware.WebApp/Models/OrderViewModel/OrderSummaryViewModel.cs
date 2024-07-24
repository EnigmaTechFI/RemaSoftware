using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class OrderSummaryViewModel
    {
        public List<Ddt_In> Ddt_In { get; set; }
        public int SubBatchId { get; set; }
        public string BasePathImages { get; set; }
    }
}
