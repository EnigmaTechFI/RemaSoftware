using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.UtilityServices.Exceptions;
using RemaSoftware.WebApp.Models.ClientViewModel;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Validation;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly ClientHelper _clientHelper;
        private readonly INotyfService _notyfToastService;
        private readonly ClientValidation _clientValidation;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ClientController(ClientHelper clientHelper, INotyfService notyfToastService, ClientValidation clientValidation)
        {
            _clientHelper = clientHelper;
            _notyfToastService = notyfToastService;
            _clientValidation = clientValidation;
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

        [HttpGet]
        public IActionResult GetAddOrUpdateClientUserModal(int clientId, string clientUserId)
        {
            if (string.IsNullOrEmpty(clientUserId))
                return PartialView("_AddUserClientModal", new AddOrdUpdateClientUserViewModel
                {
                    ParentClientId = clientId
                });

            var clientUser = _clientHelper.GetClient(clientId).ClientUsers.Single(s=>s.Id==clientUserId);
            return PartialView("_AddUserClientModal", new AddOrdUpdateClientUserViewModel
            {
                UserId = clientUserId,
                ParentClientId = clientId,
                Name = clientUser.Name,
                Surname = clientUser.Surname,
                Email = clientUser.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateClientUser(AddOrdUpdateClientUserViewModel model)
        {
            try
            {
                var validationResult = _clientValidation.ValideateClientUser(model);
                if (validationResult.Result)
                {
                    var successMessage = model.IsEdit ? "Utente modificato con successo." : "Utente aggiunto con successo.";
                    _notyfToastService.Success(successMessage);
                    await _clientHelper.AddOrUpdateUserClient(model);
                }
                else
                    _notyfToastService.Error(validationResult.Errors);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore generico durante la modifica o l'inserimento dell'utente associato al Cliente: <{model.ParentClientId}>.");
                var errorMessage = model.IsEdit
                    ? "Errore generico durante la modifica dell'utente."
                    : "Errore generico durante l'inserimento dell'utente.";
                _notyfToastService.Error(errorMessage);
            }
            
            return RedirectToAction("EditClient", new {clientId = model.ParentClientId});
        }

        // [HttpDelete]
        // public JsonResult DeleteClient(int clientId)
        // {
        //     try
        //     {
        //         var result = _clientHelper.DeleteClient(clientId);
        //         return new JsonResult(new { Result = result, ToastMessage = "Cliente eliminato con successo." });
        //         
        //     }
        //     catch (FattureInCloudException ex)
        //     {
        //         Logger.Error(ex, "Errore durante la cancellazione del Cliente su FattureInCloud.");
        //         _notyfToastService.Error("Errore durante la modifica del Cliente su FattureInCloud.");
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.Error(e, "Errore generico durante la cancellazione del Cliente con id: <{clientId}>.");
        //         _notyfToastService.Error("Errore generico durante la cancellazione del Cliente.");
        //     }
        //     return new JsonResult(new { ToastMessage = $"Errore durante l\\'eliminazione del Cliente." });
        // }

        [HttpDelete]
        public async Task<IActionResult> DeleteClientUser(string clientuserId)
        {
            try
            {
                var result = await _clientHelper.DeleteClientUser(clientuserId);
                return new JsonResult(new { Result = result, ToastMessage = "Utente eliminato con successo." });
                
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Errore generico durante la cancellazione del client user asp net con id: <{clientuserId}>.");
                _notyfToastService.Error("Errore generico durante la cancellazione dell'utente.");
            }
            return new JsonResult(new { ToastMessage = $"Errore durante l\\'eliminazione dell'utente." });
        }
    }
}