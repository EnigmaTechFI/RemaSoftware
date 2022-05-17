using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.Domain.ContextModels;
using RemaSoftware.Domain.DALServices;
using RemaSoftware.WebApp.Models.ClientViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly INotyfService _notyfToastService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ClientController(IClientService clientService, INotyfService notyfToastService)
        {
            _clientService = clientService;
            _notyfToastService = notyfToastService;
        }

        [HttpGet]
        public Task<IActionResult> AddClient()
        {
            return Task.FromResult<IActionResult>(View());
        }

        [HttpPost]
        public IActionResult AddClient(ClientViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var add_client = new Client
                    {
                        Name = model.Name,
                        StreetNumber = model.StreetNumber,
                        Street = model.Street,
                        Cap = model.Cap,
                        City = model.City,
                        Nation = model.Nation,
                        Province = model.Province,
                        P_Iva = model.P_Iva
                    };
                    _clientService.AddClient(add_client);
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