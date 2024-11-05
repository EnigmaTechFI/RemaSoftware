using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.HomeViewModel
{
    public class HomeViewModel
    {
        public int TotalCustomerCount { get; set; }
        public int TotalProcessedPieces { get; set; }
        public int TotalCountOrdersNotExtinguished { get; set; }
        public decimal LastMonthEarnings { get; set; }
        public List<ChartDataObject> AreaChartData { get; set; }
        public List<ChartDataObject> BarChartData { get; set; }
        public List<Ddt_In> OrderNearToDeadline { get; set; }
        public List<Warehouse_Stock> StockArticleAddRemQty { get; set; }
        public List<ChartDataPiecesObject> AreaPiecesChartData { get; set; } // Cambiato a ChartDataPiecesObject

        
    }

    public class ChartDataObject
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string BackgroundColor { get; set; }
    }
    
    public class ChartDataPiecesObject
    {
        public string Label { get; set; }
        public PieceData Value { get; set; }
    }
    
    public class PieceData
    {
        public int Number_Piece { get; set; }
        public int NumberMissingPiece { get; set; }
        public int NumberWastePiece { get; set; }
        public int NumberLostPiece { get; set; }
        public int NumberZama { get; set; }
        public int NumberReturnDiscard { get; set; }
        public int NumberResoPiece { get; set; }
        public int NumberFreeRepairPiece { get; set; }
    }
}
