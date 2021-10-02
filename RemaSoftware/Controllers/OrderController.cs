using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
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
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;

        public OrderController(IOrderService orderService, IPdfService pdfService, IClientService clientService, IOperationService operationService)
        {
            _orderService = orderService;
            _pdfService = pdfService;
            _clientService = clientService;
            _operationService = operationService;
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