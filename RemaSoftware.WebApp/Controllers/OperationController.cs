using Microsoft.AspNetCore.Mvc;
using System;
using NLog;
using AspNetCoreHero.ToastNotification.Abstractions;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.OperationViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    public class OperationController : Controller
    {
        private readonly IOperationService _operationService;
        private readonly INotyfService _notyfToastService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public OperationController(IOperationService operationService, INotyfService notyfToastService)
        {
            _operationService = operationService;
            _notyfToastService = notyfToastService;
        }

        [HttpGet]
        public IActionResult OperationList()
        {
            var vm = new OperationListViewModel()
            {
                Operations = _operationService.GetAllOperations()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult AddOperation()
        {
            var vm = new AddOperationViewModel();
            return View(vm);
        }
        
        [HttpGet]
        public IActionResult EditOperation(int id)
        {
            var vm = new EditOperationViewModel()
            {
                Operation = _operationService.GetOperationById(id)
            };
            return View(vm);
        }
        
        [HttpPost]
        public IActionResult EditOperation(EditOperationViewModel model)
        {
            try
            {
                _operationService.UpdateOperation(model.Operation);
                return RedirectToAction("OperationList");
            }
            catch (Exception e)
            {
                _notyfToastService.Error("Errore durante la modifica dell'operazione.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult AddOperation(AddOperationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _operationService.AddOperation(new Operation
                    {
                        Name = model.Name,
                        Description = model.Description
                    });
                    _notyfToastService.Success("Operazione aggiunta con successo.");
                    return RedirectToAction("OperationList");
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
