using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IWarehouseStockService
    {
        List<Warehouse_Stock> GetAllWarehouseStocks();
        Warehouse_Stock AddStockProduct(Warehouse_Stock warehouseStock);
        bool DeleteWarehouseStockById(int stockArticleId);
        Warehouse_Stock GetStockArticleById(int stockArticleId);
        bool UpdateStockArticle(Warehouse_Stock stockArticle);
        bool UpdateStockQuantity(Warehouse_Stock stockArticle, int quantity, int addOrRemove);
        List<Stock_History> GetReportStocks();
    }
    
}