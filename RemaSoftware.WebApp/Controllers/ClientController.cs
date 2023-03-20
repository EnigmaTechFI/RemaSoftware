using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.WebApp.Models.ClientViewModel;
using RemaSoftware.WebApp.Helper;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
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
        public async Task<IActionResult> UpdateClient(int clientId)
        {
            return View(_clientHelper.GetUpdateClientModel(clientId));
        }
        
        [HttpGet]
        public async Task<IActionResult> InfoClient(int clientId)
        {
            return View(_clientHelper.GetInfoClientModel(clientId));
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
                    return RedirectToAction("ClientList", "Client");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta del Cliente.");
                _notyfToastService.Error("Errore durante la creazione del Cliente.");
            }
            return View(model);
        }
        
        [HttpPost]
        public IActionResult UpdateClient(UpdateClientViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _clientHelper.UpdateClient(model);
                    _notyfToastService.Success("Cliente aggiornato con successo.");
                    return RedirectToAction("ClientList");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta del Cliente.");
                _notyfToastService.Error("Errore durante la creazione del Cliente.");
            }
            return View(model);
        }

        public JsonResult DeleteClient(int ClientId)
        {
            try
            {
                var deleteResult = _clientHelper.DeleteClientById(ClientId);
                return new JsonResult(new { Result = deleteResult, ToastMessage = "Cliente eliminato correttamente." });
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error delete account: {ClientId}");
                return new JsonResult(new
                    { Error = e, ToastMessage = $"Errore durante l\\'eliminazione del cliente." });
            }
        }
    }
}