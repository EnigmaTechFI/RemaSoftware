using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.DALServices;
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
    }
}