using Microsoft.AspNetCore.Mvc;
using System;
using NLog;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.OperationViewModel;
using RemaSoftware.WebApp.Validation;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
    public class OperationController : Controller
    {
        private readonly IOperationService _operationService;
        private readonly OperationValidation _operationValidation;
        private readonly INotyfService _notyfToastService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public OperationController(IOperationService operationService, INotyfService notyfToastService, OperationValidation operationValidation)
        {
            _operationService = operationService;
            _notyfToastService = notyfToastService;
            _operationValidation = operationValidation;
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
                _operationValidation.ValidateNewOperation(model.Operation);
                _operationService.UpdateOperation(model.Operation);
                return RedirectToAction("OperationList");
            }
            catch (Exception e)
            {
                _notyfToastService.Error(e.Message);
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
                    var op = new Operation
                    {
                        Name = model.Name,
                        Description = model.Description
                    };
                    _operationValidation.ValidateNewOperation(op);
                    _operationService.AddOperation(op);
                    _notyfToastService.Success("Operazione aggiunta con successo.");
                    return RedirectToAction("OperationList");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta dell'Operazione.");
                _notyfToastService.Error(ex.Message);
            }
            return View(model);
        }
    }
}
