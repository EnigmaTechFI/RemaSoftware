﻿using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.EmployeeViewModel;
using RemaSoftware.WebApp.Validation;


namespace RemaSoftware.WebApp.Controllers
{
    
    public class EmployeeController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly AccountHelper _accountHelper;
        private readonly EmployeeHelper _employeeHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly EmployeeValidation _employeeValidation;

        public EmployeeController(INotyfService notyfService, EmployeeHelper employeeHelper, EmployeeValidation employeeValidation, AccountHelper accountHelper)
        {
            _employeeHelper = employeeHelper;
            _notyfService = notyfService;
            _employeeValidation = employeeValidation;
            _accountHelper = accountHelper;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public IActionResult NewEmployee()
        {
            try
            {
                return View(_employeeHelper.NewEmployee());
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return RedirectToAction("EmployeeList", "Employee");
            }
        }
        
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> NewEmployee(EmployeeViewModel model)
        {
            DateTime today = DateTime.Now;

            model.Employee.BirthDate = (model.Employee.BirthDate.Value.Date == today.Date) ? (DateTime?)null : model.Employee.BirthDate;
            model.Employee.StartRelationship = (model.Employee.StartRelationship.Value.Date == today.Date) ? (DateTime?)null : model.Employee.StartRelationship;
            model.Employee.EndRelationship = (model.Employee.EndRelationship.Value.Date == today.Date) ? (DateTime?)null : model.Employee.EndRelationship;
            
            try 
            {
                var validationResult = _employeeValidation.ValidateEmployee(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                } else
                {
                    if (model.Employee.Mail != null)
                    {
                        MyUser myUser= await _accountHelper.AddEmployeeAccount(model);
                        model.Employee.AccountId = myUser.Id;
                    }
                    await _employeeHelper.NewEmployee(model.Employee);
                    _notyfService.Success("Impiegato aggiunto correttamente.");
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception e) {
                Logger.Error(e, $"Errore durante l'aggiunta dell'impiegato.");
                _notyfService.Error("Errore durante l'aggiunta dell'impiegato.");
            }
            return View();
        }
        
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public IActionResult EmployeeList()
        {
            try
            {
                return View(_employeeHelper.GetEmployeeListViewModel());
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return View();
            }
        }
        
        [Authorize(Roles = Roles.Admin)]
        public JsonResult DeleteEmployee(int employeeId)
        {
            try
            {
                _employeeHelper.DeleteEmployee(employeeId);
                return new JsonResult(new { Result = true, ToastMessage = "Impiegato eliminato correttamente." });
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error deleting employee: {employeeId}");
                return new JsonResult(new { Error = e, ToastMessage = "Errore durante l'eliminazione dell'impiegato." });
            }
        }

        
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public IActionResult ViewEmployee(int id, int mouth, int year)
        {
            try
            {
                return View(_employeeHelper.GetEmployeeById(id, mouth, year));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Impossibile visualizzare l'impiegato.");
                return RedirectToAction("EmployeeList");
            }
        }
        
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public IActionResult ModifyEmployee(int id)
        {
            try
            {
                return View(_employeeHelper.GetEmployeeById(id, 0, 0));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Impossibile modificare il dipendente.");
                return RedirectToAction("EmployeeList");
            }
        }
        
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ModifyEmployee(EmployeeViewModel model)
        {
            try {
                var validationResult = _employeeValidation.ValidateEmployee(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                }
                else
                {
                    await _employeeHelper.EditEmployee(model);
                    _notyfService.Success("Impiegato aggiornato correttamente");
                    return RedirectToAction("EmployeeList");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante l&#39;aggiornamento dell'impiegato.");
            }
            return View(_employeeHelper.GetEmployeeById(model.Employee.EmployeeID, 0, 0));
        }
    }
}