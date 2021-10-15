using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using RemaSoftware.DALServices;
using RemaSoftware.Helper;
using RemaSoftware.Models.ClientViewModel;
using UtilityServices;
using Microsoft.AspNetCore.Hosting;
using RemaSoftware.Models.Common;
using RemaSoftware.Models.OrderViewModel;
using Rotativa.AspNetCore;
using UtilityServices.Dtos;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloud;
        private readonly INotyfService _notyfService;
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;
        private readonly IImageService _imageService;
        private readonly PdfHelper _pdfHelper;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IClientService clientService, IOperationService operationService, PdfHelper pdfHelper, IImageService imageService, IAPIFatturaInCloudService apiFatturaInCloudService, INotyfService notyfService)
        {
            _orderService = orderService;
            _clientService = clientService;
            _operationService = operationService;
            _pdfHelper = pdfHelper;
            _imageService = imageService;
            _apiFatturaInCloud = apiFatturaInCloudService;
            _notyfService = notyfService;
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

        public IActionResult DownloadPdfOrder(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderWithOperationsById(orderId);
                
                return new ViewAsPdf("../Pdf/SingleOrderSummary", order);
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
            //vm.OldOrders = _orderService.GetAllOrders();
            vm.OldOrders = new List<string>{"sku001", "sku001", "sku001", "sku001"};
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
        public async Task<IActionResult> NewOrder(NewOrderViewModel model)
        {
            var rootpath = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;

            var guid = _imageService.SavingOrderImage(model.Photo, rootpath);

            model.Order.DataIn = DateTime.Now;

            model.Order.Image_URL = guid;

            model.Order.Price_Tot = model.Order.Price_Uni * model.Order.Number_Piece;

            model.Order.Client = _clientService.GetClient(model.Order.ClientID);

            var order_operationID = new List<int>();

            foreach (var id in model.Operation)
            {
                if (id.Flag)
                {
                    order_operationID.Add(id.Operation.OperationID);
                }
            }

            //Aggiunta Ordine DB
            var order = _orderService.AddOrder(model.Order);

            if (order != null)
            {
                //Collegamento Ordine - Operazioni DB
                _orderService.AddOrderOperation(order.OrderID, order_operationID);

                //API Fattura In Cloud
                _apiFatturaInCloud.AddOrderCloud(new OrderDto
                {
                    Name = order.Name,
                    Description = order.Description,
                    DataIn = order.DataIn,
                    DataOut = order.DataOut,
                    Number_Piece = order.Number_Piece,
                    Price_Uni = order.Price_Uni,
                    Price_Tot = order.Price_Tot,
                    SKU = order.SKU
                });

                try
                {
                    return new ViewAsPdf("../Pdf/SingleOrderSummary", order);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Errore durante la generazione del pdf.");
                }
                _notyfService.Success("Ordine creato correttamente.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Do Something
            }

            _notyfService.Error("Errore durante la creazione dell\\'ordine");
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult GetEditOrderOperationsModal(int orderId)
        {
            var vm = new EditOrderOperationsViewModel();
            
            var allAvailableOperations = _operationService.GetAllOperations();
            var order = _orderService.GetOrderWithOperationsById(orderId);
            if (order == null)
            {
                Logger.Error($"GetOperationsForEdit - order not found for id: {orderId}");
                return new JsonResult(new { Result = false, ToastMessage="Ordine non trovato."});
            }
            
            foreach (var op in allAvailableOperations)
            {
                vm.OrderOperations.Add(new OperationFlag
                {
                    Operation = op,
                    Flag = order.Order_Operation.Any(a=>a.OperationID == op.OperationID)
                });
            }

            vm.OrderId = orderId;
            return PartialView("_EditOrderOperationsModal", vm);
        }

        public JsonResult EditOrderOperations(EditOrderOperationsViewModel model)
        {
            try
            {
                var order = _orderService.GetOrderWithOperationsById(model.OrderId);

                var operationToAdd = new List<int>();
                var operationToRemove = new List<int>();
                
                foreach (var operation in model.OrderOperations)
                {
                    var existingOper = order.Order_Operation.SingleOrDefault(sd => sd.OperationID == operation.Operation.OperationID);

                    if (existingOper == null && operation.Flag) // se non esiste e flag è a true vuol dire che è nuova
                    {
                        operationToAdd.Add(operation.Operation.OperationID); continue;
                    }

                    if (existingOper != null && !operation.Flag) // se esiste ci sono 2 casi: flag=true tutto rimane uguale, flag=false si elimina
                        operationToRemove.Add(operation.Operation.OperationID);
                }
                
                var result = _operationService.EditOrderOperations(model.OrderId, operationToAdd, operationToRemove);
                return new JsonResult(new { Result = result, ToastMessage="Operazioni modificate correttamente."});
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante la modifica delle operazioni dell'ordine: {model.OrderId}.");
                return new JsonResult(new { Result = false, ToastMessage="Errore durante la modifica delle operazioni dell'ordine."});
            }


        }

    }
}