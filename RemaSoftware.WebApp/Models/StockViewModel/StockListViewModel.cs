using System;
using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.StockViewModel
{
    public class StockListViewModel
    {
        public List<Warehouse_Stock> WarehouseStocks { get; set; }
        public bool newProduct { get; set; }
    }
}