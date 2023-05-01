using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Fluent;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.StockViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
    public class StockController : Controller
    {
        private readonly StockHelper _stockHelper;
        private readonly INotyfService _notyfService;
        private readonly SupplierHelper _supplierHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public StockController(StockHelper stockHelper, INotyfService notyfService, SupplierHelper supplierHelper)
        {
            _stockHelper = stockHelper;
            _notyfService = notyfService;
            _supplierHelper = supplierHelper;
        }

        [HttpGet]
        public IActionResult Stock()
        {
            try
            {
                return View(_stockHelper.GetStockListViewModel());
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return View();
            }
        }
        
        [HttpGet]
        public IActionResult AddProduct()
        {
            try
            {
                return View(_stockHelper.GetAddProductViewModel());
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return RedirectToAction("Stock");
            }
        }

        [HttpPost]
        public IActionResult AddProduct(NewStockViewModel model)
        {
            try 
            {
                _stockHelper.AddStockProduct(model);
                _notyfService.Success("Articolo aggiunto al magazzino correttamente.");
                return RedirectToAction("Stock", "Stock"); 
            }
            catch (Exception e) {
                
                Logger.Error(e, $"Errore durante l'aggiunta dell'articolo di magazzino.");
                _notyfService.Error("Errore durante l'aggiunta dell'articolo di magazzino.");
            }
            return View(model);
        }

        public JsonResult DeleteStockArticle(int stockArticleId)
        {
            try
            {
                _stockHelper.DeleteStockArticle(stockArticleId);
                return new JsonResult(new { Result = true, ToastMessage="Articolo di magazzino eliminato correttamente."});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error deleting stockArticle: {stockArticleId}");
                return new JsonResult(new {Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo di magazzino."});
            }
        }

        [HttpPost]
        public JsonResult AddOrRemoveQuantity(QtyAddRemoveJsDTO model)
        {
            try
            {
                var result = _stockHelper.AddOrRemoveQuantityFromArticle(model);
                return new JsonResult(new { result.Result, result.ToastMessage, NewQty=result.Number_Piece, NewPrice = result.Price_Tot});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore, impossibile modificare quantità.");
                return new JsonResult(new {Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo dal magazzino."});
            }
        }
        
        [HttpGet]
        public IActionResult ViewStock(int id)
        {
            try
            {
                return View(_stockHelper.GetStockById(id));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Impossibile visualizzare il prodotto.");
                return RedirectToAction("Stock");
            }
        }
        
        [HttpGet]
        public IActionResult ModifyStock(int id)
        {
            try
            {
                return View(_stockHelper.GetStockById(id));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Impossibile modificare il prodotto.");
                return RedirectToAction("Stock");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ModifyStock(StockViewModel model)
        {
            try {
                await _stockHelper.EditStock(model);
                _notyfService.Success("Prodotto aggiornato correttamente");
                return RedirectToAction("Stock", "Stock");
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante l&#39;aggiornamento del prodotto.");
                return View(_stockHelper.GetStockById(model.WarehouseStock.Warehouse_StockID));
            }
        }
        
        [HttpGet]
        public IActionResult RetireProduct()
        {
            try
            {
                return View(_stockHelper.GetStockListViewModel());
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return View();
            }
        }
    }
}
