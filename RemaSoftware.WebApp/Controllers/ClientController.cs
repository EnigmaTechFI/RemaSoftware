using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Exceptions;
using RemaSoftware.WebApp.Models.ClientViewModel;
using RemaSoftware.WebApp.Helper;

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
        public async Task<IActionResult> AddClient(ClientViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _clientHelper.AddClient(model);
                    _notyfToastService.Success("Cliente aggiunto con successo.");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (FattureInCloudException ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta del Cliente su FattureInCloud.");
                _notyfToastService.Error("Errore durante la creazione del Cliente su FattureInCloud.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore generico durante l'aggiunta del Cliente.");
                _notyfToastService.Error("Errore generico durante l'aggiunta del Cliente.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditClient(int clientId)
        {
            var vm = _clientHelper.GetClient(clientId);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditClient(ClientViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _clientHelper.EditClient(model);
                    _notyfToastService.Success("Cliente modificato con successo.");
                    return View(model);
                }
            }
            catch (FattureInCloudException ex)
            {
                Logger.Error(ex, "Errore durante la modifica del Cliente su FattureInCloud.");
                _notyfToastService.Error("Errore durante la modifica del Cliente su FattureInCloud.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore generico durante la modifica del Cliente.");
                _notyfToastService.Error("Errore generico durante la modifica del Cliente.");
            }
            return View(model);
        }

        [HttpDelete]
        public JsonResult DeleteClient(int clientId)
        {
            try
            {
                var result = _clientHelper.DeleteClient(clientId);
                return new JsonResult(new { Result = result, ToastMessage = "Cliente eliminato con successo." });
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l\\'eliminazione del Cliente con id: <{clientId}>.");
                return new JsonResult(new { Error = ex, ToastMessage = $"Errore durante l\\'eliminazione del Cliente." });
            }
        }
    }
}