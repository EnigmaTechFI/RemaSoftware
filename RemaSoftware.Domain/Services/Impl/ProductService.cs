using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProductService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Product AddProduct(Product product)
        {
            _dbContext.Add(product);
            _dbContext.SaveChanges();
            return product;
        }

        public List<string> GetAllProductSKU()
        {
            return _dbContext.Products.Select(s => s.SKU).ToList();
        }

        public List<Product> GetAllProducts()
        {
            return _dbContext.Products.Include(i => i.Client).Include(s => s.Ddts_In).ToList();
        }

        public Product GetProductById(int productId)
        {
            return _dbContext.Products.Include(i => i.Client).Include(s => s.Ddts_In).SingleOrDefault(i => i.ProductID == productId);
        }

        public string DeleteProduct(Product product)
        {
            try
            {
                _dbContext.Remove(product);
                _dbContext.SaveChanges();

                return "Success";
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'eliminazione del prodotto: #{product.SKU}");
                return e.Message;
            }
        }

        public Product UpdateProduct(Product product)
        {
            try
            {
                var updatedProduct = _dbContext.Update(product);
                _dbContext.SaveChanges();

                return updatedProduct.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento del prodotto: #{product.SKU}");
            }
            return null;
        }
    }
}
