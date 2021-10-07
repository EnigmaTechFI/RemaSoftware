using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemaSoftware.Models.HomeViewModel
{
    public class HomeViewModel
    {
        public int TotalCustomerCount { get; set; }
        public int TotalProcessedPieces { get; set; }
        public int TotalCountOrdersNotExtinguished { get; set; }
    }
}
