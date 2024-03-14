using System;
using System.Globalization;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.WebApp.Helper;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.PriceViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
    public class PriceController : Controller
    {
        private readonly PriceHelper _priceHelper;
        private readonly IOperationService _operationService;
        private readonly ProductHelper _productHelper;
        private readonly INotyfService _notyfToastService;
        private readonly IConfiguration _configuration;

        public PriceController(PriceHelper priceHelper, ProductHelper productHelper, IOperationService operationService, INotyfService notyfToastService, IConfiguration configuration)
        {
            _priceHelper = priceHelper;
            _productHelper = productHelper;
            _operationService = operationService;
            _notyfToastService = notyfToastService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult PriceList()
        {
            return View(_priceHelper.GetPriceListViewModel());
        }
        
        [HttpGet]
        public IActionResult NewPrice()
        {
            var vm = new NewPriceViewModel
            {
                Products = _productHelper.GetAllProducts(),
                Operations = _operationService.GetAllOperations()
            };

            return View(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> NewPrice(NewPriceViewModel model)
        {
            try
            {
                var price = await _priceHelper.NewPrice(model);
                _notyfToastService.Success("Prezzo aggiunto correttamente");
                return RedirectToAction("PriceList", "Price");
            }
            catch(Exception ex)
            {
                model.Products = _productHelper.GetAllProducts();
                model.Operations = _operationService.GetAllOperations();
                return View(model);
            }
        }
        
        [HttpGet]
        public IActionResult ViewPrice(int id)
        {
            var vm = new PriceViewModel
            {
                Price = _priceHelper.GetPriceById(id)
            };

            return View(vm);
        }
        
        [HttpGet]
        public void DeletePrice(int priceId)
        {
            try
            {
                _priceHelper.DeletePrice(priceId);
                _notyfToastService.Success("Prodotto prezzo correttamente");
            }
            catch(Exception ex)
            {
                _notyfToastService.Error(ex.Message);
            }
        }
        
        [HttpGet]
        public IActionResult UpdatePrice(int priceId)
        {
            var vm = new NewPriceViewModel
            {
                Price = _priceHelper.GetPriceById(priceId)
            };
            vm.PriceVal = vm.Price.PriceVal.ToString("N", new CultureInfo("it-IT"));
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(NewPriceViewModel model)
        {
            try
            {
                await _priceHelper.UpdatePrice(model);
                _notyfToastService.Success("Prezzo aggiornato correttamente");
                return RedirectToAction("PriceList");
            }
            catch (Exception ex)
            {
                _notyfToastService.Error(ex.Message);
                return View(model);
            }
        }
    }
}
