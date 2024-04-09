using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
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
        private readonly UserManager<MyUser> _userManager;


        public AttendanceController(INotyfService notyfService, UserManager<MyUser> userManager, EmployeeHelper employeeHelper, AttendanceHelper attendanceHelper)
        {
            _employeeHelper = employeeHelper;
            _notyfService = notyfService;
            _attendanceHelper = attendanceHelper;
            _userManager = userManager;
        }
        
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Attendance(int month, int year, bool NotUpdate)
        {
            try
            {
                if (!NotUpdate)
                {
                    await _attendanceHelper.UpdateAttendance(month, year);
                }
                return View(_employeeHelper.GetAttendanceViewModel(month, year));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore, impossibile procedere.");
                return View();
            }
        }
        
        [Authorize(Roles = Roles.Impiegato)]
        [HttpGet]
        public async Task<IActionResult> AttendanceEmployee(int month, int year)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                int EmployeeId = _employeeHelper.GetEmployeeId(user.Id);

                await _attendanceHelper.UpdateAttendanceEmployee(month, year, EmployeeId);
                return View(_employeeHelper.GetAttendanceEmployeeViewModel(month, year, EmployeeId));
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
                byte[] pdfBytes = Convert.FromBase64String(pdfData);

                _attendanceHelper.SendAttendance(month, year, mail, pdfBytes);

                _notyfService.Success("Invio completato.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'invio delle presenze.");
                Logger.Error("Errore durante l'aggiunta della presenza.");
            }
        }
        
        [Authorize(Roles = Roles.Impiegato)]
        [HttpPost]
        public void SendNoAttendance(int EmployeeId, string note)
        {
            try
            {
                _attendanceHelper.SendNoAttendance(EmployeeId, note);
                _notyfService.Success("Invio completato.");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante l'invio.");
                Logger.Error("Errore durante l'invio.");
            }
        }
    }
}