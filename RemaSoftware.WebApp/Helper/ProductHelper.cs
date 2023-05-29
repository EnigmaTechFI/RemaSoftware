using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.ProductViewModel;
using RemaSoftware.WebApp.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.UtilityServices.Interface;

namespace RemaSoftware.WebApp.Helper
{
    public class ProductHelper
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly ProductValidation _productValidation;

        public ProductHelper(IProductService productService, IImageService imageService, IConfiguration configuration, ProductValidation productValidation)
        {
            _productService = productService;
            _imageService = imageService;
            _configuration = configuration;
            _productValidation = productValidation;
        }

        public Product GetProductById(int productId)
        {
            return _productService.GetProductById(productId);
        }

        public async Task<string> UpdateProduct(UpdateProductViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    model.Product.FileName = await _imageService.SavingOrderImage(model.Photo);
                }
                _productValidation.ValidateProduct(model.Product);
                _productService.UpdateProduct(model.Product);
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public UpdateProductViewModel UpdateProduct(int productId)
        {
            var product = GetProductById(productId);
            return new UpdateProductViewModel()
            {
                Product = product,
                BasePathImages = $"{_configuration["ApplicationUrl"]}{_configuration["ImagesEndpoint"]}order/",
            };
        }

        public async Task<Product> AddProduct(NewProductViewModel model)
        {
            var products = _productService.GetAllProductSKU();
            if (products.Contains(model.Product.SKU))
            {
                throw new Exception("Prodotto già registrato.");
            }
            if (!string.IsNullOrEmpty(model.Photo))
            {
                model.Product.FileName = await _imageService.SavingOrderImage(model.Photo);
            }
            var validation = _productValidation.ValidateProduct(model.Product);
            if(validation != "")
                throw new Exception(validation);
            return _productService.AddProduct(model.Product);

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

        public ProductListViewModel GetProductListViewModel()
        {
            return new ProductListViewModel
            {
                Products = GetAllProducts()
            };
        }
    }
}
