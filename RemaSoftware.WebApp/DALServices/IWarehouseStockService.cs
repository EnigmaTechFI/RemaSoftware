using System.Collections.Generic;
using RemaSoftware.WebApp.ContextModels;

namespace RemaSoftware.WebApp.DALServices
{
    public interface IWarehouseStockService
    {
        List<Warehouse_Stock> GetAllWarehouseStocks();
        bool AddOrUpdateWarehouseStock(Warehouse_Stock stockArticle);
        bool DeleteWarehouseStockById(int stockArticleId);
        Warehouse_Stock GetStockArticleById(int stockArticleId);
        bool UpdateStockArticle(Warehouse_Stock stockArticle);
        bool UpdateQtyByArticleId(int articleId, int qtyToAdd);
    }
}