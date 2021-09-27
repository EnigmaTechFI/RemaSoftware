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
        public IActionResult AddClient()
        {
            var vm = new ClientViewModel();

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddClient(ClientViewModel model)
        {
           
            return View(model);
        }


        [HttpGet]
        public IActionResult NewOrder()
        {
            var vm = new NewOrderViewModel();

            return View(vm);
        }

        [HttpPost]
        public IActionResult NewOrder(NewOrderViewModel model)
        {

            return View(model);
        }



        [HttpGet]
        public IActionResult OrderSummary()
        {
            var vm = new OrderSummaryViewModel();

            return View(vm);
        }

        [HttpPost]
        public IActionResult OrderSummary(OrderSummaryViewModel model)
        {
            return View(model);
        }



    }
}