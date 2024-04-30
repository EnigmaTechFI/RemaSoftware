using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services.Impl
{
    public class PriceService : IPriceService
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PriceService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Price> GetAllPrices()
        {
            return _dbContext.Prices.Include(t => t.Product).ThenInclude(y => y.Client).Include(o => o.PriceOperation).ThenInclude(p => p.Operation).ToList();
        }
        
        public List<Price> GetAllPricesByProductId(int productId)
        {
            return _dbContext.Prices
                .Include(t => t.Product).ThenInclude(y => y.Client)
                .Include(o => o.PriceOperation).ThenInclude(p => p.Operation)
                .Where(p => p.Product.ProductID == productId)
                .ToList();
        }
        
        public List<Price> GetPrice(int productId, List<Operation> operations)
        {

            var operationIds = operations.Select(op => op.OperationID).ToList();
    
            return _dbContext.Prices
                .Include(t => t.Product).ThenInclude(y => y.Client)
                .Include(o => o.PriceOperation).ThenInclude(p => p.Operation)
                .Where(p => p.Product.ProductID == productId && operationIds.All(id => p.PriceOperation.Any(po => po.OperationID == id)))
                .ToList();
        }
        
        public Price NewPrice(Price price)
        {
            _dbContext.Add(price);
            _dbContext.SaveChanges();
            return price;
        }
        
        public Price GetPriceById(int Id)
        {
            return _dbContext.Prices
                .Include(p => p.Product).ThenInclude(y => y.Client)
                .Include(p => p.PriceOperation).ThenInclude(y => y.Operation)
                .SingleOrDefault(p => p.PriceID == Id);
        }
        
        public Price GetPriceAndPriceOperation(int Id)
        {
            return _dbContext.Prices
                .Include(p => p.PriceOperation)
                .SingleOrDefault(p => p.PriceID == Id);
        }
        
        public string DeletePrice(Price price)
        {
            try
            {
                _dbContext.Remove(price);
                _dbContext.SaveChanges();

                return "Success";
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'eliminazione del prezzo.");
                return e.Message;
            }
        }
        
        public Price UpdatePrice(Price price)
        {
            try
            {
                var updatedPrice = _dbContext.Update(price);
                _dbContext.SaveChanges();

                return updatedPrice.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento del prezzo: #{price.PriceID}");
            }
            return null;
        }
        
        
    }
}
