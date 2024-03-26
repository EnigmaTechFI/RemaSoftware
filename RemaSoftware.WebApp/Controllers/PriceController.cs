using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;
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
                Products = _productHelper.GetAllProducts().OrderBy(r => r.SKU).ToList(),
                Operations = _operationService.GetAllOperations().OrderBy(t => t.Name.ToString()).ToList()
            };

            return View(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> NewPrice(NewPriceViewModel model)
        {
            try
            {
                model.Operations = _operationService.GetAllOperations();
                model.Price.PriceOperation = new List<PriceOperation>();
                
                List<int> selectedOperationIDs = model.SelectedOperationIDs.Split(',')
                    .Select(int.Parse)
                    .ToList();

                foreach (var operationId in selectedOperationIDs)
                {
                    var operation = new PriceOperation();
                    
                    var selectedOperation = model.Operations.FirstOrDefault(op => op.OperationID == operationId);
                    if (selectedOperation != null)
                    {
                        operation.OperationID = selectedOperation.OperationID;
                        model.Price.PriceOperation.Add(operation);
                    }
                }
                
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
