using System;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
    public class EmployeeController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly EmployeeHelper _employeeHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EmployeeController(INotyfService notyfService, EmployeeHelper employeeHelper)
        {
            _employeeHelper = employeeHelper;
            _notyfService = notyfService;
        }

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
                return RedirectToAction("Stock", "Stock", new { newProduct = false });
            }
        }
    }
}