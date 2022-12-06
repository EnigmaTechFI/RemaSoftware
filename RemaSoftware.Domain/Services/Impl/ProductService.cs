using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (product == null)
                throw new Exception("Product vuoto.");
            try
            {
                var newProduct = _dbContext.Add(product);
                _dbContext.SaveChanges();

                return newProduct.Entity;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiunta del prodotto: {product.ToString()}");
            }
            return null;
        }

        public List<Product> GetAllProducts()
        {
            return _dbContext.Products.Include(i => i.Client).Include(s => s.Ddt_In).ToList();
        }

        public Product GetProductById(int productId)
        {
            return _dbContext.Products.Include(i => i.Client).SingleOrDefault(i => i.ProductID == productId);
        }
    }
}
