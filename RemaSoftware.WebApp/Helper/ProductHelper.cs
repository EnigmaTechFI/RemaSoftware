using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices;
using RemaSoftware.WebApp.Models.ProductViewModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace RemaSoftware.WebApp.Helper
{
    public class ProductHelper
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly string _basePathForImages;

        public ProductHelper(IProductService productService, IImageService imageService, IConfiguration configuration)
        {
            _productService = productService;
            _imageService = imageService;
            _configuration = configuration;
            _basePathForImages =
                (Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName + _configuration["ImagePath"]).Replace("/", "\\");
        }

        public Product AddProduct(NewProductViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    var imageName = _imageService.SavingOrderImage(model.Photo, _basePathForImages);
                    model.Product.Image_URL = imageName;
                }
                var addedOrder = _productService.AddProduct(model.Product);
                return addedOrder;
            }
            catch(Exception ex)
            {
                throw new Exception();
            }

        }

        public List<Product> GetAllProducts()
        {
            try
            {
                return _productService.GetAllProducts();
                
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        public string DeleteProduct(int productId)
        {
            try
            {
                var product = _productService.GetProductById(productId);
                return _productService.DeleteProduct(product);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}
