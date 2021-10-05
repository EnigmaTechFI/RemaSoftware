using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.StockViewModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.DALServices;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWarehouseStockService _warehouseService;

        public StockController(ApplicationDbContext applicationDbContext, IWarehouseStockService warehouseService)
        {
            this._applicationDbContext = applicationDbContext;
            _warehouseService = warehouseService;
        }
        
        [HttpGet]
        public IActionResult Stock()
        {
            var vm = new StockListViewModel
            {
                WarehouseStocks = _warehouseService.GetAllWarehouseStocks()
            };

            return View(vm);
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
