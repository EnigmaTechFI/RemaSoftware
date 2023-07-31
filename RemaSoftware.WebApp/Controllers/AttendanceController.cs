using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers
{
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
        
        [Authorize(Roles = Roles.Admin)]
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
        
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
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
        
        [Authorize(Roles = Roles.Admin)]
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
        
        [Authorize(Roles = Roles.Admin)]
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
        
        [Authorize(Roles = Roles.Admin)]
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

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public void SendAttendance(int month, int year, string mail, string pdfData)
        {
            try
            {
                // Decodifica il contenuto Base64 del PDF in byte[]
                byte[] pdfBytes = Convert.FromBase64String(pdfData);

                // Esegui l'elaborazione del PDF come desiderato (invio via email, ecc.)
                _attendanceHelper.SendAttendance(month, year, mail, pdfBytes);

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