using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.DALServices;
using RemaSoftware.Models.ClientViewModel;
using UtilityServices;
using UtilityServices.Dtos;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPdfService _pdfService;

        public OrderController(IOrderService orderService, IPdfService pdfService)
        {
            _orderService = orderService;
            _pdfService = pdfService;
        }
        
        [HttpGet]
        public IActionResult OrderSummary()
        {
            var orders = _orderService.GetAllOrders();
            var vm = new OrderSummaryViewModel
            {
                Orders = orders
            };
            
            return View(vm);
        }

        public FileResult DownloadPdfOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            // todo crea vista ordinePdf
            var fileBytes = _pdfService.GeneratePdf("");
            return File(fileBytes, "application/pdf");
        }
    }
}