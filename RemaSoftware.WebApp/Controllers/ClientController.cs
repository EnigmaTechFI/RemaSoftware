using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.Models.ClientViewModel;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.ClientViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly INotyfService _notyfToastService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ClientController(ClientHelper clientHelper, INotyfService notyfToastService)
        {
            _clientHelper = clientHelper;
            _notyfToastService = notyfToastService;
        }

        [HttpGet]
        public async Task<IActionResult> ClientList()
        {
            var vm = new ClientListViewModel()
            {
                Clients = _clientHelper.GetAllClients()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddClient()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddClient(ClientViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _clientHelper.AddClient(model);
                    _notyfToastService.Success("Cliente aggiunto con successo.");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta del Cliente.");
                _notyfToastService.Error("Errore durante la creazione del Cliente.");
            }
            return View(model);
        }
    }
}