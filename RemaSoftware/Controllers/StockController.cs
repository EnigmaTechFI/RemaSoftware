using System;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Models.StockViewModel;
using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.DALServices;
using Microsoft.AspNetCore.Authorization;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly IWarehouseStockService _warehouseService;
        private readonly INotyfService _notyfToastService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public StockController(IWarehouseStockService warehouseService, INotyfService notyfToastService)
        {
            _warehouseService = warehouseService;
            _notyfToastService = notyfToastService;
        }
        
        [HttpGet]
        public IActionResult Stock()
        {
            var stocks = _warehouseService.GetAllWarehouseStocks();
            var vm = new StockListViewModel
            {
                WarehouseStocks = stocks.Select(s=>new StockViewModel
                {
                    StockArticleId = s.Warehouse_StockID,
                    Name = s.Name,
                    Brand = s.Brand,
                    Size = s.Size,
                    Price_Uni = s.Price_Uni,
                    Number_Piece = s.Number_Piece
                }).ToList()
            };

            return View(vm);
        }
        
        [HttpGet]
        public IActionResult AddProduct()
        {
            var vm = new StockViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddProduct(StockViewModel model)
        {
            try { 
                if (ModelState.IsValid)
                {
                    model.Size = string.IsNullOrEmpty(model.Size)
                        ? "Unica"
                        : model.Size;
                    _warehouseService.AddOrUpdateWarehouseStock(new Warehouse_Stock
                    {
                        Name = model.Name,
                        Brand = model.Brand,
                        Size = model.Size,
                        Number_Piece = model.Number_Piece,
                        Price_Uni = model.Price_Uni
                    });
                    return RedirectToAction("Stock", "Stock"); //redirect to "Giacenze"
                }
            }
            catch (DbUpdateException) {
                ModelState.AddModelError("", "Unable to save changes. " +
                   "Try again, and if the problem persists " +
                   "see your system administrator.");
            }
            return View(model);
        }

 
        [HttpPost]
        public JsonResult SaveStockArticleEdit(Warehouse_Stock model)
        {
            try
            {
                var result = _warehouseService.AddOrUpdateWarehouseStock(model);
                return new JsonResult(new {result});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore durante l'aggiornamento dell'articolo magazzino: {model.Warehouse_StockID}");
                return new JsonResult(new {Error = e, Messge = $"Errore durante l\'aggiornamento dell\'articolo magazzino: {model.Warehouse_StockID}"});
            }
            return null;
        }
        
        public JsonResult DeleteStockArticle(int stockArticleId)
        {
            try
            {
                var deleteResult = _warehouseService.DeleteWarehouseStockById(stockArticleId);
                return new JsonResult(new { Result = deleteResult, ToastMessage="Articolo di magazzino eliminato correttamente."});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error deleting stockArticle: {stockArticleId}");
                return new JsonResult(new {Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo di magazzino."});
            }
        }

        [HttpPost]
        public JsonResult AddOrRemoveQuantity(QtyAddRemoveJSModel model)
        {
            if (model.ArticleId <= default(int))
                return new JsonResult(new {Result = false, ToastMessage = $"Articolo mancante."});
            if (model.QtyToAddRemove <= 0)
                return new JsonResult(new {Result = false, ToastMessage = $"La quantità deve essere maggiore di 0."});
            
            try
            {
                var stockArticle = _warehouseService.GetStockArticleById(model.ArticleId);
                
                if(model.QtyToAddRemoveRadio == 0 && stockArticle.Number_Piece - model.QtyToAddRemove < 0)
                    return new JsonResult(new {Result = false, ToastMessage = $"La quantità risulterebbe minore di 0."});
                
                stockArticle.Number_Piece = model.QtyToAddRemoveRadio == 1
                    ? stockArticle.Number_Piece + model.QtyToAddRemove
                    : stockArticle.Number_Piece - model.QtyToAddRemove;
                var res = _warehouseService.UpdateStockArticle(stockArticle);
                return new JsonResult(new { Result = res, ToastMessage="Quantità modificata correttamente.", NewQty=stockArticle.Number_Piece, NewPrice = stockArticle.Price_Tot});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error modifing quantity of stockArticle: {model.ArticleId}");
                return new JsonResult(new {Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo di magazzino."});
            }
        }
    }

    public class QtyAddRemoveJSModel
    {
        public int ArticleId { get; set; }
        public int QtyToAddRemove { get; set; }
        public int QtyToAddRemoveRadio { get; set; }
    }
}
