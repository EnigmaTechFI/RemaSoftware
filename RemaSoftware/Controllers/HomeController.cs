using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models;
using RemaSoftware.Models.HomeViewModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RemaSoftware.DALServices;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IOrderService _orderService;

        public HomeController(IClientService clientService, IOrderService orderService)
        {
            _clientService = clientService;
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new HomeViewModel
            {
                TotalCustomerCount = _clientService.GetTotalCustomerCount(),
                TotalProcessedPieces = _orderService.GetTotalProcessedPiecese(),
                TotalCountOrdersNotExtinguished = _orderService.GetCountOrdersNotExtinguished(),
                LastMonthEarnings = _orderService.GetLastMonthEarnings()
            };
            return View(vm);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
