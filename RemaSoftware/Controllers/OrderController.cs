using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.DALServices;
using RemaSoftware.Helper;
using RemaSoftware.Models.ClientViewModel;
using UtilityServices;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPdfService _pdfService;
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;
        private readonly PdfHelper _pdfHelper;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IPdfService pdfService, IClientService clientService, IOperationService operationService, PdfHelper pdfHelper)
        {
            _orderService = orderService;
            _pdfService = pdfService;
            _clientService = clientService;
            _operationService = operationService;
            _pdfHelper = pdfHelper;
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

        public async Task<FileResult> DownloadPdfOrder(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderById(orderId);
                var vieString = await _pdfHelper.RenderViewToString("Pdf/SingleOrderSummary", order);
                var fileBytes = _pdfService.GeneratePdf(vieString);
                
                return File(fileBytes, "application/pdf");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Errore durante la generazione del pdf.");
            }

            return null;
        }
        
        
        [HttpGet]
        public IActionResult NewOrder()
        {
            var vm = new NewOrderViewModel();
            vm.Clients = _clientService.GetAllClients();
            vm.Operation = new List<OperationFlag>();
            var oper = _operationService.GetAllOperations();
            foreach (var op in oper)
            {
                vm.Operation.Add(new OperationFlag
                {
                    Operation = op,
                    Flag = false
                });
            }

            return View(vm);
        }
        
        
        [HttpPost]
        public IActionResult NewOrder(NewOrderViewModel model)
        {
            string source = model.Photo;
            string base64 = source.Substring(source.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);
            var guid = Guid.NewGuid().ToString();
            var file = "wwwroot/img/" + guid +".png";

            System.IO.File.WriteAllBytes(file, data);

            model.Order.DataIn = DateTime.Now;

            model.Order.Image_URL = file;

            //TODO API, PDF, DATA_OUT, READ_OERATION
            _orderService.AddOrder(model.Order);

            return RedirectToAction("Index", "Home");
        }
    }
}