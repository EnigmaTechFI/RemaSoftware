using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.ClientViewModel;
using Convert = System.Convert;
using MemoryStream = System.IO.MemoryStream;
using Image = System.Drawing;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public ClientController(UserManager<MyUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            this._applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AddClient()
        {


            var vm = new ClientViewModel();

            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddClient(ClientViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var add_client = new Client { Name = model.Client.Name, StreetNumber = model.Client.StreetNumber, Street = model.Client.Street, Cap = model.Client.Cap, City = model.Client.City, State = model.Client.State, P_Iva = model.Client.P_Iva };
                    _applicationDbContext.Add(add_client);
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
           
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> NewOrder()
        {
            var vm = new NewOrderViewModel();

            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            vm.Clients = _applicationDbContext.Clients.ToList();

            vm.Operation = _applicationDbContext.Operations.ToList();

            return View(vm);
        }

        [HttpPost]
        public IActionResult NewOrder(NewOrderViewModel model)
        {
            string source = model.Photo;
            string base64 = source.Substring(source.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);
            var guid = Guid.NewGuid().ToString();
            var file = "wwwroot/img/" + guid +".png";

            System.IO.File.WriteAllBytes(file, data);

            model.Order.DataIn = DateTime.Now;
            model.Order.DataOut = DateTime.Now;

            model.Order.Image_URL = file;

            //TODO API, PDF, DATA_OUT, READ_OERATION

            _applicationDbContext.Add(model.Order);
            _applicationDbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public async Task<IActionResult> OrderSummary()
        {
            var vm = new OrderSummaryViewModel();

            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            vm.Username = user.UserName;

            return View(vm);
        }

        [HttpPost]
        public IActionResult OrderSummary(OrderSummaryViewModel model)
        {
            return View(model);
        }



    }
}