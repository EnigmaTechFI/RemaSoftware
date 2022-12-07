using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices;
using RemaSoftware.WebApp.Models.ProductViewModel;
using RemaSoftware.WebApp.Validation;
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
        private readonly ClientHelper _clientHelper;
        private readonly ProductValidation _productValidation;
        private readonly string _basePathForImages;

        public ProductHelper(IProductService productService, IImageService imageService, IConfiguration configuration, ClientHelper clientHelper, ProductValidation productValidation)
        {
            _productService = productService;
            _imageService = imageService;
            _configuration = configuration;
            _basePathForImages =
                (Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName + _configuration["ImagePath"]).Replace("/", "\\");
            _clientHelper = clientHelper;
            _productValidation = productValidation;
        }

        public Product GetProductById(int productId)
        {
            return _productService.GetProductById(productId);
        }

        public string RetrieveProductPhoto(string path)
        {
            try
            {
                var photo = File.ReadAllBytes(_basePathForImages +  path);
                var base64 = Convert.ToBase64String(photo);
                return String.Format("data:image/gif;base64,{0}", base64);
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        public string UpdateProduct(UpdateProductViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    var imageName = _imageService.SavingOrderImage(model.Photo, _basePathForImages);
                    model.Product.Image_URL = imageName;
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
            var product = this.GetProductById(productId);
            return new UpdateProductViewModel()
            {
                Product = product,
                Clients = _clientHelper.GetAllClients(),
                Photo = RetrieveProductPhoto(product.Image_URL)
            };
        }

        public string AddProduct(NewProductViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    var imageName = _imageService.SavingOrderImage(model.Photo, _basePathForImages);
                    model.Product.Image_URL = imageName;
                }
                _productValidation.ValidateProduct(model.Product);
                _productService.AddProduct(model.Product);
                return "Success";
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
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
