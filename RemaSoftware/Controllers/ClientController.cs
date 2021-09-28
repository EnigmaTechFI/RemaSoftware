using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.ClientViewModel;

namespace RemaSoftware.Controllers
{
    public class ClientController : Controller
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ClientController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._applicationDbContext = applicationDbContext;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> AddClient()
        {


            var vm = new ClientViewModel();

            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            vm.Username = user.UserName;
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddClient(ClientViewModel model)
        {
           
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> NewOrder()
        {
            var vm = new NewOrderViewModel();

            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            vm.Username = user.UserName;

            return View(vm);
        }

        [HttpPost]
        public IActionResult NewOrder(NewOrderViewModel model)
        {

            return View(model);
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