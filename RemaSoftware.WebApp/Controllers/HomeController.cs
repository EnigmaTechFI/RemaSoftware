using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models;
using RemaSoftware.WebApp.Models.HomeViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize(Roles = Roles.Admin +"," + Roles.DipendenteControl +"," + Roles.Dipendente)]
    public class HomeController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IOrderService _orderService;
        private readonly DashboardHelper _dashboardHelper;
        private readonly IWarehouseStockService _stockService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public HomeController(IClientService clientService, IOrderService orderService, DashboardHelper dashboardHelper, IWarehouseStockService stockService)
        {
            _clientService = clientService;
            _orderService = orderService;
            _dashboardHelper = dashboardHelper;
            _stockService = stockService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new HomeViewModel
            {
                TotalCustomerCount = _clientService.GetTotalCustomerCount(),
                TotalProcessedPieces = _orderService.GetTotalProcessedPiecese(),
                TotalCountOrdersNotExtinguished = _orderService.GetCountOrdersNotExtinguished(),
                LastMonthEarnings = _orderService.GetLastMonthEarnings(),
                AreaChartData = _dashboardHelper.GetDataForDashboardAreaChart(),
                OrderNearToDeadline = _orderService.GetOrdersNearToDeadlineTakeTop(5),
                StockArticleAddRemQty = _dashboardHelper.GetAllWarehouseStocksForDashboard(),
                BarChartData = _dashboardHelper.GetDataForBarChartDashboard(),
                AreaPiecesChartData = _dashboardHelper.GetDataForDashboardAreaChartPieces(),
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
