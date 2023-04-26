using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl
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
            return _dbContext.Warehouse_Stocks.Include(i => i.Supplier).ToList();
        }

        public Warehouse_Stock AddStockProduct(Warehouse_Stock warehouse_Stock)
        {
            try
            {
                _dbContext.Add(warehouse_Stock);
                _dbContext.Stock_Histories.Add(new Stock_History()
                {
                    Entry = true,
                    Number_Piece = warehouse_Stock.Number_Piece,
                    Warehouse_StockID = warehouse_Stock.Warehouse_StockID,
                    Date = DateTime.Now,
                    Warehouse_Stock = warehouse_Stock
                });
                _dbContext.SaveChanges();
                return warehouse_Stock;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante la creazione del prodotto.");
                throw new Exception($"Errore durante la creazione del prodotto.");
            }
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
                articleToUpdate.Number_Piece = stockArticle.Number_Piece;
                articleToUpdate.Price_Uni = stockArticle.Price_Uni;
                articleToUpdate.SupplierID = stockArticle.SupplierID;
                articleToUpdate.Reorder_Limit = stockArticle.Reorder_Limit;
                articleToUpdate.Measure_Unit = stockArticle.Measure_Unit;
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
            return _dbContext.Warehouse_Stocks.Include(t => t.Stock_Histories).Include(i => i.Supplier).SingleOrDefault(sd => sd.Warehouse_StockID == stockArticleId);
        }

        public bool UpdateStockArticle(Warehouse_Stock stockArticle)
        {
            try
            {
                _dbContext.Warehouse_Stocks.Update(stockArticle);
                _dbContext.SaveChanges();
                return true;
            }catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento del prodotto: #{stockArticle.Warehouse_StockID}");
                throw new Exception($"Errore durante l'aggiornamento del prodotto: #{stockArticle.Warehouse_StockID}");
            }
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