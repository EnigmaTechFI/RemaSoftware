using System;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.EmployeeViewModel;
using RemaSoftware.WebApp.Validation;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
    public class AttendanceController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly EmployeeHelper _employeeHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AttendanceController(INotyfService notyfService, EmployeeHelper employeeHelper)
        {
            _employeeHelper = employeeHelper;
            _notyfService = notyfService;
        }
        
        [HttpGet]
        public IActionResult Attendance(int mouth, int year)
        {
            try
            {
                return View(_employeeHelper.GetAttendanceViewModel(mouth, year));
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