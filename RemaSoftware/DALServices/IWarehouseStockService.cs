using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IWarehouseStockService
    {
        List<Warehouse_Stock> GetAllWarehouseStocks();
        bool AddOrUpdateWarehouseStock(Warehouse_Stock stockArticle);
        bool DeleteWarehouseStockById(int stockArticleId);
    }
}