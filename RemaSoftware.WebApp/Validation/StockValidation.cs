using RemaSoftware.WebApp.Models.StockViewModel;

namespace RemaSoftware.WebApp.Validation
{
    public class StockValidation
    {
        public string ValidateStock(NewStockViewModel warehouseStock)
        {

            if (string.IsNullOrEmpty(warehouseStock.WarehouseStock.Name))
                return "Inserire nome prodotto.";
            if (warehouseStock.WarehouseStock.Number_Piece<=0)
                return "Inserire numero di pezzi del prodotto.";
            if (string.IsNullOrEmpty(warehouseStock.WarehouseStock.Product_Code))
                return"Inserire codice prodotto.";
            if (warehouseStock.WarehouseStock.SupplierID <= 0)
                return"Inserire fornitore.";
            if (warehouseStock.WarehouseStock.Reorder_Limit <= 0)
                return "Inserire limite per il riordino.";
            return "";
        }
        
        public string ValidateStockModify(StockViewModel warehouseStock)
        {

            if (string.IsNullOrEmpty(warehouseStock.WarehouseStock.Name))
                return "Inserire nome prodotto.";
            if (warehouseStock.WarehouseStock.Number_Piece<=0)
                return "Inserire numero di pezzi del prodotto.";
            if (string.IsNullOrEmpty(warehouseStock.WarehouseStock.Product_Code))
                return"Inserire codice prodotto.";
            if (warehouseStock.WarehouseStock.Reorder_Limit <= 0)
                return "Inserire limite per il riordino.";
            return "";
        }
    }
}
