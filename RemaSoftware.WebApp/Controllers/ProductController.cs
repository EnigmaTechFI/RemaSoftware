﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.ProductViewModel;
using System;
using System.Threading.Tasks;
using It.FattureInCloud.Sdk.Model;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using Product = RemaSoftware.Domain.Models.Product;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente +"," + Roles.DipendenteControl)]
    public class ProductController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly ProductHelper _productHelper;
        private readonly INotyfService _notyfToastService;
        private readonly IConfiguration _configuration;

        public ProductController(ClientHelper clientHelper, ProductHelper productHelper, INotyfService notyfToastService, IConfiguration configuration)
        {
            _clientHelper = clientHelper;
            _productHelper = productHelper;
            _notyfToastService = notyfToastService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult ProductList()
        {
            return View(_productHelper.GetProductListViewModel());
        }

        [HttpGet]
        public IActionResult NewProduct()
        {
            var vm = new NewProductViewModel
            {
                Clients = _clientHelper.GetAllClients()
            };

            return View(vm);
        }
        
        [HttpGet]
        public IActionResult ViewProduct(int id)
        {
            var vm = new ProductViewModel
            {
                Product = _productHelper.GetProductById(id),
                BasePathImages = $"{_configuration["ApplicationUrl"]}{_configuration["ImagesEndpoint"]}order/"
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> NewProduct(NewProductViewModel model)
        {
            try
            {
                var product = await _productHelper.AddProduct(model);
                _notyfToastService.Success("Prodotto aggiunto correttamente");
                return RedirectToAction("NewOrder", "Order", new{ productId = product.ProductID});
            }
            catch(Exception ex)
            {
                model.Clients = _clientHelper.GetAllClients();
                _notyfToastService.Error(ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public void DeleteProduct(int productId)
        {
            try
            {
                _productHelper.DeleteProduct(productId);
                _notyfToastService.Success("Prodotto eliminato correttamente");
            }
            catch(Exception ex)
            {
                _notyfToastService.Error(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult UpdateProduct(int productId)
        {
            return View(_productHelper.UpdateProduct(productId));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductViewModel model)
        {
            try
            {
                await _productHelper.UpdateProduct(model);
                _notyfToastService.Success("Prodotto aggiornato correttamente");
                return RedirectToAction("ProductList");
            }
            catch (Exception ex)
            {
                _notyfToastService.Error(ex.Message);
                return View(model);
            }
        }
        
        public Product GetPiecesById(int productId)
        {
            try
            {
                return _productHelper.GetPiecesById(productId);
            }
            catch(Exception ex)
            {
                _notyfToastService.Error(ex.Message);
            }

            return null;
        }
    }
}
