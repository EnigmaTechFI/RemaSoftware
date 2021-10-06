using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using RemaSoftware.ContextModels;
using RemaSoftware.DALServices;
using RemaSoftware.Data;
using RemaSoftware.Models.ClientViewModel;
using Convert = System.Convert;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ApplicationDbContext _applicationDbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
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
                    var add_client = new Client { Name = model.Client.Name, StreetNumber = model.Client.StreetNumber, Street = model.Client.Street, Cap = model.Client.Cap, City = model.Client.City, State = model.Client.State, P_Iva = model.Client.P_Iva };
                    _clientService.AddClient(add_client);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Errore durante l'aggiunta del Cliente.");
            }
           
            return View(model);
        }



    }
}