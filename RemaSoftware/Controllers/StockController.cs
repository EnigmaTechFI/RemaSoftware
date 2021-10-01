using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.StockViewModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public StockController(UserManager<MyUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            this._applicationDbContext = applicationDbContext;
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
        
        [HttpGet]
        public IActionResult AddProduct()
        {
            var vm = new StockViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddProduct(StockViewModel model)
        {
            try { 
                if (ModelState.IsValid)
                {
                    model.Warehouse_Stock.Price_Tot = model.Warehouse_Stock.Price_Uni * model.Warehouse_Stock.Number_Piece;
                    _applicationDbContext.Add(model.Warehouse_Stock);
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Stock", "Stock"); //redirect to "Giacenze"
                }
            }
            catch (DbUpdateException) {
                ModelState.AddModelError("", "Unable to save changes. " +
                   "Try again, and if the problem persists " +
                   "see your system administrator.");
            }
            return View(model);
        }

    }
}
