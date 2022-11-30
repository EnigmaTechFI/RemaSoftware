using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RemaSoftware.UtilityServices;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.ProductViewModel;
using System;
using System.IO;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly ProductHelper _productHelper;

        public ProductController(ClientHelper clientHelper, ProductHelper productHelper)
        {
            _clientHelper = clientHelper;
            _productHelper = productHelper;
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
                if(ModelState.IsValid)
                    _productHelper.AddProduct(model);
                /*TODO: Effettuare tutti i controlli necessari*/
                return View(model);
            }
            catch(Exception ex)
            {
                return View(model);
            }
        }
    }
}
