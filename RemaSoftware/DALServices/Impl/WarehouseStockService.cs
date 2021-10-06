using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        public bool AddOrUpdateWarehouseStock(Warehouse_Stock stockArticle)
        {
            if (stockArticle == null)
                throw new ArgumentException("AddOrUpdateWarehouseStock invocata con parametro stockArticle a null.");
            if (stockArticle.Warehouse_StockID == 0)
            {
                _dbContext.Add(stockArticle);
            }
            else
            {
                var articleToUpdate = _dbContext.Warehouse_Stocks
                    .SingleOrDefault(sd => sd.Warehouse_StockID == stockArticle.Warehouse_StockID);
                if (articleToUpdate == null)
                    throw new Exception($"Stock article not found with id: {stockArticle.Warehouse_StockID}");
                    
                
                articleToUpdate.Name = stockArticle.Name;
                articleToUpdate.Brand = stockArticle.Brand;
                articleToUpdate.Size = stockArticle.Size;
                articleToUpdate.Number_Piece = stockArticle.Number_Piece;
                articleToUpdate.Price_Uni = stockArticle.Price_Uni;
                articleToUpdate.Price_Tot = stockArticle.Price_Tot;
                _dbContext.Update(articleToUpdate);
            }

            _dbContext.SaveChanges();
            return true;
        }
    }
}