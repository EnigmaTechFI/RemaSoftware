using System;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public StockController(StockHelper stockHelper, INotyfService notyfService)
        {
            _stockHelper = stockHelper;
            _notyfService = notyfService;
        }
        
        [HttpGet]
        public IActionResult Stock()
        {
            var vm = new StockListViewModel();
            try
            {
                vm = _stockHelper.GetStockListViewModel();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante il caricamento della lista di magazzino.");
                _notyfService.Error("Errore durante il caricamento della lista di magazzino.");
            }
            
            return View(vm);
        }
        
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View(new StockViewModel());
        }

        [HttpPost]
        public IActionResult AddProduct(StockViewModel model)
        {
            try 
            { 
                if (ModelState.IsValid)
                {
                    _stockHelper.AddStockProduct(model);
                    
                    _notyfService.Success("Articolo aggiunto al magazzino correttamente.");
                    return RedirectToAction("Stock", "Stock"); //redirect to "Giacenze"
                }
            }
            catch (Exception e) {
                
                Logger.Error(e, $"Errore durante l'aggiunta dell'articolo di magazzino.");
                _notyfService.Error("Errore durante l'aggiunta dell'articolo di magazzino.");
            }
            return View(model);
        }

 
        // non usata
        [HttpPost]
        public JsonResult SaveStockArticleEdit(Warehouse_Stock model)
        {
            try
            {
                _stockHelper.EditStockArticle(model);
                return new JsonResult(new {result = true, ToastMessage="Articolo di magazzino modificato correttamente."});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento dell'articolo magazzino: {model.Warehouse_StockID}");
                return new JsonResult(new {Error = e, Messge = $"Errore durante l\'aggiornamento dell\'articolo magazzino: {model.Warehouse_StockID}"});
            }
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
                Logger.Error(e, $"Error modifing quantity of stockArticle: {model.ArticleId}");
                return new JsonResult(new {Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo di magazzino."});
            }
        }
    }
}
