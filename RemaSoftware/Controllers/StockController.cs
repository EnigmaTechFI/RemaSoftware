using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.StockViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemaSoftware.Controllers
{
    public class StockController : Controller
    {

        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public StockController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._applicationDbContext = applicationDbContext;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Stock()
        {
            var vm = new StockViewModel();

            var a = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(a);

            vm.Username = user.UserName;
            return View(vm);
        }

        [HttpPost]
        public IActionResult Stock(StockViewModel model)
        {
            return View(model);
        }
    }
}
