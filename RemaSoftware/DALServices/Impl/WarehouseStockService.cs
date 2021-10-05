using System.Collections.Generic;
using System.Linq;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;

namespace RemaSoftware.DALServices.Impl
{
    public class WarehouseStockService : IWarehouseStockService
    {
        private readonly ApplicationDbContext _dbContext;

        public WarehouseStockService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public List<Warehouse_Stock> GetAllWarehouseStocks()
        {
            return _dbContext.Warehouse_Stocks.ToList();
        }
    }
}