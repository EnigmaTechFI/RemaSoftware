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
            _dbContext.Warehouse_Stocks.Update(stockArticle);
            _dbContext.SaveChanges();
            return true;
        }
        
        public bool UpdateStockQuantity(Warehouse_Stock stockArticle, int quantity, int addOrRemove)
        {
            bool AddOrRemove = addOrRemove != 0;

            try
            {
                _dbContext.Stock_Histories.Add(new Stock_History(){
                    Entry = AddOrRemove,
                    Number_Piece = quantity,
                    Warehouse_StockID = stockArticle.Warehouse_StockID,
                    Date = DateTime.Now,
                    Warehouse_Stock = stockArticle
                });
                _dbContext.Warehouse_Stocks.Update(stockArticle);
                _dbContext.SaveChanges();
                return true;
            }catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento del prodotto: #{stockArticle.Warehouse_StockID}");
                throw new Exception($"Errore durante l'aggiornamento del prodotto: #{stockArticle.Warehouse_StockID}");
            }
        }

    }
}