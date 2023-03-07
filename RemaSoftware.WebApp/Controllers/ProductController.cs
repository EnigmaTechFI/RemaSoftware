using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.ProductViewModel;
using System;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly ProductHelper _productHelper;
        private readonly INotyfService _notyfToastService;
        private readonly SubBatchHelper _subBatchHelper;

        public ProductController(ClientHelper clientHelper, ProductHelper productHelper, INotyfService notyfToastService, SubBatchHelper subBatchHelper)
        {
            _clientHelper = clientHelper;
            _productHelper = productHelper;
            _notyfToastService = notyfToastService;
            _subBatchHelper = subBatchHelper;
        }

        [HttpGet]
        public IActionResult ProductList(int? subBatchId)
        {
            var vm = new ProductListViewModel
            {
                SubBatch = subBatchId != null ? _subBatchHelper.GetSubBatchDetail(subBatchId.Value) : new SubBatch(){SubBatchID = 0},
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
