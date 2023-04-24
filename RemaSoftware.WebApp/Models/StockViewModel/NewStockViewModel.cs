using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.StockViewModel
{
    public class NewStockViewModel
    {

        public Warehouse_Stock WarehouseStock { get; set; }
        public List<Supplier> Suppliers { get; set; }

    }
}
