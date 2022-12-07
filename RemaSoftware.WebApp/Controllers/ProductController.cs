using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.ProductViewModel;
using System;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly ProductHelper _productHelper;
        private readonly INotyfService _notyfToastService;

        public ProductController(ClientHelper clientHelper, ProductHelper productHelper, INotyfService notyfToastService)
        {
            _clientHelper = clientHelper;
            _productHelper = productHelper;
            _notyfToastService = notyfToastService;
        }

        [HttpGet]
        public IActionResult ProductList()
        {
            var vm = new ProductListViewModel
            {
                Products = _productHelper.GetAllProducts()
            };

            return View(vm);
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

        [HttpPost]
        public IActionResult NewProduct(NewProductViewModel model)
        {
            try
            {
                _productHelper.AddProduct(model);
                _notyfToastService.Success("Prodotto aggiunto correttamente");
                return RedirectToAction("ProductList");
            }
            catch(Exception ex)
            {
                model.Clients = _clientHelper.GetAllClients();
                _notyfToastService.Error(ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult DeleteProduct(int productId)
        {
            try
            {
                return new JsonResult(new { Result = _productHelper.DeleteProduct(productId), ToastMessage = "Prodotto eliminato correttamente." });
            }
            catch(Exception e)
            {
                return new JsonResult(new { Result = "Error", ToastMessage = e.Message });
            }
            
        }

        [HttpGet]
        public IActionResult UpdateProduct(int productId)
        {
            return View(_productHelper.UpdateProduct(productId));
        }

        [HttpPost]
        public IActionResult UpdateProduct(UpdateProductViewModel model)
        {
            try
            {
                _productHelper.UpdateProduct(model);
                _notyfToastService.Success("Prodotto aggiornato correttamente");
                return RedirectToAction("ProductList");
            }
            catch (Exception ex)
            {
                model.Clients = _clientHelper.GetAllClients();
                _notyfToastService.Error(ex.Message);
                return View(model);
            }
        }

    }
}
