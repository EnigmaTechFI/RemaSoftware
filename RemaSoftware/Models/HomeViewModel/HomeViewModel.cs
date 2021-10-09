using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Models.HomeViewModel
{
    public class HomeViewModel
    {
        public int TotalCustomerCount { get; set; }
        public int TotalProcessedPieces { get; set; }
        public int TotalCountOrdersNotExtinguished { get; set; }
        public decimal LastMonthEarnings { get; set; }
        public List<ChartDataObject> PieChartData { get; set; }
        public List<ChartDataObject> AreaChartData { get; set; }
        public List<Order> OrderNearToDeadline { get; set; }
        public List<Warehouse_Stock> StockArticleAddRemQty { get; set; }
        
    }

    public class ChartDataObject
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string BackgroundColor { get; set; }
    }
}
