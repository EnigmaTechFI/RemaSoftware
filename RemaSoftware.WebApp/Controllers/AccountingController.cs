using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreHero.ToastNotification.Abstractions;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.AccountingViewModel;

namespace RemaSoftware.WebApp.Controllers
{        
    [Authorize(Roles = Roles.Admin)]
    public class AccountingController : Controller
    {
        private readonly AccountingHelper _accountingHelper;
        private readonly INotyfService _notyfService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AccountingController(AccountingHelper accountingHelper, INotyfService notyfService)
        {
            _accountingHelper = accountingHelper;
            _notyfService = notyfService;
        }

        [HttpGet]
        public IActionResult Preliminar()
        {
            try
            {
                return View(_accountingHelper.GetPreliminarViewModel());
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult ProductionAnalysisLive()
        {
            return View(_accountingHelper.GetProductionAnalysisLiveViewModel());
        }
        
        [HttpGet]
        public IActionResult Accounting()
        {
            var vm = new AccountingViewModel();
            try
            {
                vm = _accountingHelper.GetAccountingViewModel();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante il caricamento della Contabilità.");
                _notyfService.Error("Errore durante il caricamento della Contabilità.");
            }
            return View(vm);
        }

        public IActionResult DownloadPdfAccounting()
        {
            var vm = new AccountingViewModel();
            try
            {
                vm = _accountingHelper.GetAccountingViewModel();
                return View("../Pdf/Accounting", vm);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante la generazione del pdf.");
                _notyfService.Error("Errore durante la generazione del pdf.");
                return RedirectToAction("Accounting");
            }
        }
        
        [HttpGet]
        public IActionResult DdtVariation()
        {
            try
            {
                return View(_accountingHelper.GetDdtVariationViewModel());
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore nel caricamento dei lotti.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public JsonResult ConfirmVariation(int id)
        {
            try
            {
                _accountingHelper.ConfirmVariation(id);
                return Json(new { Result = true, Message = "Prezzo ddt aggiornato correttamente."});
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = "Errore aggiornamento DDT." });
            }
        }

        [HttpGet]
        public IActionResult DeleteVariation(int id)
        {
            try
            {
                _accountingHelper.DeleteVariation(id);
                return Json(new { Result = true, Message = "Variazione ddt annullata."});
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = "Errore aggiornamento DDT. Si prega di riprovare" });
            }
        }

        [HttpGet]
        public IActionResult RequestVariation(int id, string price, string mail, string message)
        {
            try
            {
                _accountingHelper.RequestVariation(id, price, mail, message);
                return Json(new { Result = true, Message = "Richiesta inviata."});
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult BatchAnalysis(int id)
        {
            try
            {
                return View(_accountingHelper.GetBatchAnalsysisViewModel(id));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Preliminar");
            }
        }
        
        [HttpGet]
        public IActionResult AccountingClient(int id)
        {
            try
            {
                return View(_accountingHelper.GetClientAnalsysisViewModel(id));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Preliminar");
            }
        }
        
        [HttpGet]
        public ActionResult GetPiecesForPeriod(int selectedMonth, int selectedYear, int id)
        {
            int totalPieces = _accountingHelper.getPieces(selectedMonth, selectedYear, id);

            return Json(totalPieces);
        }
        
        [HttpGet]
        public ActionResult GetPriceForPeriod(int selectedMonth, int selectedYear, int id)
        {
            decimal totalPrice = _accountingHelper.getPrice(selectedMonth, selectedYear, id);

            return Json(totalPrice);
        }
        
    }
}