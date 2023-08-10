using System;
using System.Threading.Tasks;
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
    
    public class MachineController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly MachineHelper _machineHelper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MachineController(INotyfService notyfService, MachineHelper machineHelper)
        {
            _machineHelper = machineHelper;
            _notyfService = notyfService;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public IActionResult AutomaticMachine()
        {
            try
            {
                return View(_machineHelper.AutomaticMachine());
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