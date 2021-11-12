using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RemaSoftware.DALServices;
using RemaSoftware.Helper;
using RemaSoftware.Models.ClientViewModel;
using UtilityServices;
using RemaSoftware.Models.Common;
using RemaSoftware.Models.OrderViewModel;
using RemaSoftware.Models.PDFViewModel;
using Microsoft.Extensions.Configuration;
using RemaSoftware.ContextModels;
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
        private readonly IConfiguration _configuration;
        private readonly string _basePathForImages;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IClientService clientService, IOperationService operationService, PdfHelper pdfHelper, IImageService imageService, IAPIFatturaInCloudService apiFatturaInCloudService, INotyfService notyfService, IConfiguration configuration)
        {
            _orderService = orderService;
            _clientService = clientService;
            _operationService = operationService;
            _pdfHelper = pdfHelper;
            _imageService = imageService;
            _apiFatturaInCloud = apiFatturaInCloudService;
            _notyfService = notyfService;
            _configuration = configuration;
            _basePathForImages = 
                (Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName + _configuration["ImagePath"]).Replace("/", "\\");
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
            var vm = new PDFViewModel();
                
            var order = _orderService.GetOrderWithOperationsById(orderId);
            vm.Order = order;
            if (!string.IsNullOrEmpty(order.Image_URL))
            {
                var path = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName + _configuration["ImagePath"] + order.Image_URL;
                try
                {
                    var photo = System.IO.File.ReadAllBytes(path);
                    var base64 = Convert.ToBase64String(photo);
                    var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
                    vm.Photo = imgSrc;
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Immagine non trovata. Path: {path}");
                    _notyfService.Error("Errore durante la generazione del pdf.");
                    return RedirectToAction("OrderSummary");
                }
            } 
            return View("../Pdf/SingleOrderSummary", vm);
        }
        
        
        [HttpGet]
        public IActionResult NewOrder()
        {
            var availableOperations = _operationService.GetAllOperations();
            var vm = new NewOrderViewModel
            {
                Clients = _clientService.GetAllClients(),
                OldOrders_SKU = _orderService.GetOldOrders_SKU().Distinct().ToList(),
                Operations = availableOperations?.Select(s=>new OperationFlag
                {
                    Operation = s,
                    Flag = false
                }).ToList()
            };
            vm.OldOrders_SKU.Insert(0, "");

            vm.RedirectUrlAfterCreation = Url.Action("Index", "Home");
            return View(vm);
        }

        [HttpPost]
        public JsonResult NewOrder(NewOrderViewModel model)
        {
            DateTime parsedDateTime;
            DateTime.TryParseExact(model.DataOutStr, "dd/MM/yyyy", null, DateTimeStyles.None, out parsedDateTime);
            model.Order.DataOut = parsedDateTime;

            var validationResult = this.ValidateNewOrderViewModel(model);
            if (validationResult != "")
                return new JsonResult(new {Result = false, ToastMessage = validationResult});

            if (!string.IsNullOrEmpty(model.Photo))
            {
                var iamgeName = _imageService.SavingOrderImage(model.Photo, _basePathForImages);
                model.Order.Image_URL = iamgeName;
            }

            model.Order.DataIn = DateTime.UtcNow;
            model.Order.Price_Uni = (decimal)model.uni_price;

            // aggiungo all'ordine le operazioni selezionate
            var operationsSelected = model.Operations.Where(w=>w.Flag).ToList();
            model.Order.Order_Operation = operationsSelected.Select(s => new Order_Operation
            {
                OperationID = s.Operation.OperationID
            }).ToList();
            
            var order = _orderService.AddOrder(model.Order);
            
            //API Fattura In Cloud
            try
            {
                var id_fatture = _apiFatturaInCloud.AddOrderCloud(new OrderDto
                {
                    Name = order.Name,
                    Description = order.Description,
                    DataIn = order.DataIn,
                    DataOut = order.DataOut,
                    Number_Piece = order.Number_Piece,
                    Price_Uni = order.Price_Uni,
                    SKU = order.SKU,
                    DDT = order.DDT
                });

                order.ID_FattureInCloud = id_fatture;

                _orderService.UpdateOrder(order);
            }
            catch (Exception e)
            {
                _orderService.DeleteOrderByID(order.OrderID);
                Logger.Error(e, "Errore salvataggio fatture in cloud.");
            } 
            return new JsonResult(new {Result = true, Data = order.OrderID});
        }

        private string ValidateNewOrderViewModel(NewOrderViewModel model)
        {
            if (string.IsNullOrEmpty(model.Order.SKU))
                return "SKU mancante.";
            if (string.IsNullOrEmpty(model.Order.DDT))
                return "Codice DDT mancante.";
            if (string.IsNullOrEmpty(model.Photo) && string.IsNullOrEmpty(model.Order.Image_URL))
                return "Foto mancante.";
            if (model.Order.ClientID <= default(int))
                return "Cliente mancante.";
            if (string.IsNullOrEmpty(model.Order.Name))
                return "Nome mancante.";
            if (model.uni_price<default(decimal) || model.uni_price == null)
                return "Prezzo unitario mancante.";
            if (model.Order.DataOut<=default(DateTime))
                return "Data di scadenza non valida.";
            return "";
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

        [HttpGet]
        public IActionResult GetCopyOrderModal(int orderId)
        {
            var vm = new CopyOrderViewModel();

            vm.OrderId = orderId;
            return PartialView("_CopyOrderModal", vm);
        }

        public JsonResult SaveDuplicateOrder(CopyOrderViewModel model)
        {
            var validationResult = this.ValidateDuplicateOrderViewModel(model);
            var order = _orderService.GetOrderWithOperationsById(model.OrderId);
            Order newOrder = new Order();
            newOrder.DDT = model.Code_DDT;
            newOrder.Number_Piece = model.NumberPiece;
            newOrder.DataIn = DateTime.UtcNow;
            newOrder.ClientID = order.ClientID;
            newOrder.DataOut = order.DataOut;
            newOrder.Description = order.Description;
            newOrder.Image_URL = order.Image_URL;
            newOrder.Name = order.Name;
            newOrder.Note = order.Note;
            newOrder.SKU = order.SKU;
            newOrder.Price_Uni = order.Price_Uni;

            // aggiungo all'ordine le operazioni selezionate
            var operationsSelected = order.Order_Operation.ToList();

            newOrder.Order_Operation = operationsSelected.Select(s => new Order_Operation
            {
                OperationID = s.OperationID
            }).ToList();

            var duplicateOrder = _orderService.AddOrder(newOrder);

            //API Fattura In Cloud
            try
            {
                var id_fatture = _apiFatturaInCloud.AddOrderCloud(new OrderDto
                {
                    Name = duplicateOrder.Name,
                    Description = duplicateOrder.Description,
                    DataIn = duplicateOrder.DataIn,
                    DataOut = duplicateOrder.DataOut,
                    Number_Piece = duplicateOrder.Number_Piece,
                    Price_Uni = duplicateOrder.Price_Uni,
                    SKU = duplicateOrder.SKU,
                    DDT = duplicateOrder.DDT
                });

                duplicateOrder.ID_FattureInCloud = id_fatture;

                _orderService.UpdateOrder(duplicateOrder);

                var result = true;
                return new JsonResult(new { Result = result, ToastMessage = "Ordine duplicato con successo" });
            }
            catch (Exception e)
            {
                _orderService.DeleteOrderByID(duplicateOrder.OrderID);
                Logger.Error($"Errore durante la duplicazione dell'ordine: {model.OrderId}.");
                return new JsonResult(new { Result = false, ToastMessage = "Errore durante la duplicazione dell'ordine." });
            }

        }

        private string ValidateDuplicateOrderViewModel(CopyOrderViewModel model)
        {
            if (string.IsNullOrEmpty(model.Code_DDT))
                return "Codice DDT mancante.";
            return "";
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

        
        public JsonResult GetOrderBySKU(string orderSKU)
        {
            var order = _orderService.GetOrderBySKU(orderSKU);

            if (order == null || orderSKU == null)
            {
                return new JsonResult(new {ToastMessage = $"Errore durante il recupero dell\\'ordine." });
            }
            
            return new JsonResult(new {Result = true, Data = order});
        }

        public ActionResult GetImageByOrderId(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            
            try
            {
                var fileByte = System.IO.File.ReadAllBytes(_basePathForImages + order.Image_URL);
                return File(fileByte, "image/png");
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
    }
}