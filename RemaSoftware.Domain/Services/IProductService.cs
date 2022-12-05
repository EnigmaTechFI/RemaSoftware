using RemaSoftware.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemaSoftware.Domain.Services
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product AddProduct(Product product);
        Product GetProductById(int productId);
    }
}
