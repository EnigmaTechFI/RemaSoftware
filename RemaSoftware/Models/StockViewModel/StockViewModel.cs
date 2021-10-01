using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Models.StockViewModel
{
    public class StockViewModel
    {
        public string Username { get; set; }

        public Warehouse_Stock Warehouse_Stock { get; set; }
    }
}
