using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel
{
    public class OrderControlViewModel
    {
        public List<Ddt_In> Ddt_In { get; set; }
        public string BasePathImages { get; set; }
    }
}