using System;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
    public class AttendanceController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly EmployeeHelper _employeeHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AttendanceHelper _attendanceHelper;

        public AttendanceController(INotyfService notyfService, EmployeeHelper employeeHelper, AttendanceHelper attendanceHelper)
        {
            _employeeHelper = employeeHelper;
            _notyfService = notyfService;
            _attendanceHelper = attendanceHelper;
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
        
        public void DeleteAttendance(int attendanceId)
        {
            try
            {
                _attendanceHelper.DeleteAttendance(attendanceId);
                _notyfService.Success("Eliminazione completata.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'eliminazione della presenza.");
                Logger.Error("Errore durante l'eliminazione della presenza.");
            }
        }
        
        [HttpPost]
        public void ModifyAttendance(ModifyAttendanceDTO model)
        {
            try
            {
                _attendanceHelper.ModifyAttendance(model);
                _notyfService.Success("Modifica completata.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante la modifica della presenza.");
                Logger.Error("Errore durante la modifica della presenza.");
            }
        }
        
        [HttpPost]
        public void NewAttendance(ModifyAttendanceDTO model)
        {
            try
            {
                _attendanceHelper.NewAttendance(model);
                _notyfService.Success("Aggiunta completata.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'aggiunta della presenza.");
                Logger.Error("Errore durante l'aggiunta della presenza.");
            }
        }
    }
}