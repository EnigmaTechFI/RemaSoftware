using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.StockViewModel;
using RemaSoftware.WebApp.Validation;


namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Dipendente + "," + Roles.MagazzinoMaterie +"," + Roles.DipendenteControl)]
    public class StockController : Controller
    {
        private readonly StockHelper _stockHelper;
        private readonly INotyfService _notyfService;
        private readonly SupplierHelper _supplierHelper;
        private readonly StockValidation _stockValidation;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public StockController(StockHelper stockHelper, INotyfService notyfService, StockValidation stockValidation, SupplierHelper supplierHelper)
        {
            _stockHelper = stockHelper;
            _notyfService = notyfService;
            _stockValidation = stockValidation;
            _supplierHelper = supplierHelper;
        }

        [HttpGet]
        public IActionResult Stock(bool newProduct)
        {
            try
            {
                return View(_stockHelper.GetStockListViewModel(newProduct));
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
                return RedirectToAction("Stock", new { newProduct = false });
            }
        }

        [HttpPost]
        public IActionResult AddProduct(NewStockViewModel model)
        {
            try 
            {
                var validationResult = _stockValidation.ValidateStock(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                    model.Suppliers = _supplierHelper.GetAllSuppliers();
                    model.UnitMeasure = WarehouseStockMeasure.GetUnitMeasure();
                } else
                {
                    _stockHelper.AddStockProduct(model);
                    _notyfService.Success("Articolo aggiunto al magazzino correttamente.");
                    return RedirectToAction("Stock", new { newProduct = true });
                }
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
                return RedirectToAction("Stock", new { newProduct = false });
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
                return RedirectToAction("Stock", new { newProduct = false });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ModifyStock(StockViewModel model)
        {
            try {
                var validationResult = _stockValidation.ValidateStockModify(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                }
                else
                {
                    await _stockHelper.EditStock(model);
                    _notyfService.Success("Prodotto aggiornato correttamente");
                    return RedirectToAction("Stock", new { newProduct = false });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante l&#39;aggiornamento del prodotto.");
            }
            return View(_stockHelper.GetStockById(model.WarehouseStock.Warehouse_StockID));
        }
        
        [HttpGet]
        public IActionResult RetireProduct()
        {
            try
            {
                return View(_stockHelper.GetStockListViewModel(false));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return View();
            }
        }
        
        [HttpGet]
        public IActionResult ReportStock()
        {
            try
            {
                return View(_stockHelper.ReportStock());
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
