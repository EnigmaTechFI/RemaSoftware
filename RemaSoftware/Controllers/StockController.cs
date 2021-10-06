using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.StockViewModel;
using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.DALServices;

namespace RemaSoftware.Controllers
{
    public class StockController : Controller
    {
        private readonly IWarehouseStockService _warehouseService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public StockController(IWarehouseStockService warehouseService)
        {
            _warehouseService = warehouseService;
        }
        
        [HttpGet]
        public IActionResult Stock()
        {
            var vm = new StockListViewModel
            {
                WarehouseStocks = _warehouseService.GetAllWarehouseStocks()
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
                    model.Warehouse_Stock.Price_Tot = model.Warehouse_Stock.Price_Uni * model.Warehouse_Stock.Number_Piece;
                    _warehouseService.AddOrUpdateWarehouseStock(model.Warehouse_Stock);
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
            }
            return null;
        }
    }
}
