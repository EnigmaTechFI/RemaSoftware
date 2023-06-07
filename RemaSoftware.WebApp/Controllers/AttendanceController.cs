using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration;
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
        public async Task<IActionResult> Attendance(int month, int year)
        {
            try
            {
                await _attendanceHelper.UpdateAttendance(month, year);
                return View(_employeeHelper.GetAttendanceViewModel(month, year));
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
        
        [HttpPost]
        public void NewAllAttendance(ModifyAttendanceDTO model)
        {
            try
            {
                _attendanceHelper.NewAllAttendance(model);
                _notyfService.Success("Aggiunta completata.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'aggiunta della presenza.");
                Logger.Error("Errore durante l'aggiunta della presenza.");
            }
        }
        
        [HttpPost]
        public void ExportAttendance()
        {
            try
            {
                _attendanceHelper.ExportAttendance();
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'esportazione delle presenze.");
                Logger.Error("Errore durante l'aggiunta della presenza.");
            }
        }
        
        [HttpPost]
        public void SendAttendance(ModifyAttendanceDTO model)
        {
            try
            {
                //Help di creazione del txt e dell'invio
                _notyfService.Success("Invio completato.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'invio delle presenze.");
                Logger.Error("Errore durante l'aggiunta della presenza.");
            }
        }
    }
}