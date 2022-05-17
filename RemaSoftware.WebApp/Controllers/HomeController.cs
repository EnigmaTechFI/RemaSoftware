using System;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using NLog;
using RemaSoftware.WebApp.DALServices;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models;
using RemaSoftware.WebApp.Models.HomeViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    [Authorize]
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
                //PieChartData = _dashboardHelper.GetDataForDashboardPieChart(),
                AreaChartData = _dashboardHelper.GetDataForDashboardAreaChart(),
                OrderNearToDeadline = _orderService.GetOrdersNearToDeadlineTakeTop(5),
                StockArticleAddRemQty = _dashboardHelper.GetAllWarehouseStocksForDashboard(),
                BarChartData = _dashboardHelper.GetDataForBarChartDashboard()
            };
            return View(vm);
        }


        public JsonResult AddRemoveSingleQtyDashboard(int articleId, bool isAdd)
        {
            try
            {
                var result = _stockService.UpdateQtyByArticleId(articleId, isAdd ? 1 : -1);
                return new JsonResult(new {Result = result, ToastMessage = isAdd ? $"Aggiunto 1 pezzo all\\'articlo di magazzino." : $"Sottratto 1 pezzo all\\'articlo di magazzino."});
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error modifing quantity of stockArticle: {articleId}");
                return new JsonResult(new {Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo di magazzino."});
            }
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
