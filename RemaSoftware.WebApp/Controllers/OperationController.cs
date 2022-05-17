using Microsoft.AspNetCore.Mvc;
using System;
using NLog;
using AspNetCoreHero.ToastNotification.Abstractions;
using RemaSoftware.WebApp.ContextModels;
using RemaSoftware.WebApp.DALServices;
using RemaSoftware.WebApp.Data;
using RemaSoftware.WebApp.Models.OperationViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    public class OperationController : Controller
    {
        private readonly IOperationService _operationService;
        private readonly INotyfService _notyfToastService;
        private readonly ApplicationDbContext _applicationDbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public OperationController(IOperationService operationService, INotyfService notyfToastService)
        {
            _operationService = operationService;
            _notyfToastService = notyfToastService;
        }

        [HttpGet]
        public IActionResult AddOperation()
        {
            var vm = new AddOperationViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddOperation(AddOperationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var add_operation = new Operation
                    {
                        Name = model.Name,
                        Description = model.Description
                    };
                    _operationService.AddOperation(add_operation);
                    _notyfToastService.Success("Operazione aggiunta con successo.");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta dell'Operazione.");
                _notyfToastService.Error("Errore durante la creazione dell'Operazione.");
            }
            return View(model);
        }
    }
}
