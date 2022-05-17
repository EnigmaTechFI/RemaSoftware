using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using RemaSoftware.Domain.ContextModels;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.DALServices.Impl
{
    public class WarehouseStockService : IWarehouseStockService
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                _dbContext.Update(articleToUpdate);
            }

            _dbContext.SaveChanges();
            return true;
        }

        public bool DeleteWarehouseStockById(int stockArticleId)
        {
            _dbContext.Warehouse_Stocks.Remove(new Warehouse_Stock {Warehouse_StockID = stockArticleId});
            _dbContext.SaveChanges();
            return true;
        }

        public Warehouse_Stock GetStockArticleById(int stockArticleId)
        {
            return _dbContext.Warehouse_Stocks.SingleOrDefault(sd => sd.Warehouse_StockID == stockArticleId);
        }

        public bool UpdateStockArticle(Warehouse_Stock stockArticle)
        {
            _dbContext.Warehouse_Stocks.Update(stockArticle);
            _dbContext.SaveChanges();
            return true;
        }

        public bool UpdateQtyByArticleId(int articleId, int qtyToAdd)
        {
            var article = _dbContext.Warehouse_Stocks.SingleOrDefault(sd => sd.Warehouse_StockID == articleId);
            if(article == null)
                throw new Exception($"Articolo non trovato. Id: {articleId}");
            article.Number_Piece += qtyToAdd;
            _dbContext.Warehouse_Stocks.Update(article);
            _dbContext.SaveChanges();
            return true; 
        }
    }
}