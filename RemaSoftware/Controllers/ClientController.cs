using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.ClientViewModel;
using Convert = System.Convert;

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



    }
}