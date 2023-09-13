using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IProductService
    {
        List<Product> GetAllProducts();
        Product AddProduct(Product product);
        Product GetProductById(int productId);
        Product GetProductAndOrderById(int productId);
        string DeleteProduct(Product product);
        Product UpdateProduct(Product product);
        public List<string> GetAllProductSKU();
    }
}
