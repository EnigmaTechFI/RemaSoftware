using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using NLog;
using RemaSoftware.DALServices;
using RemaSoftware.Helper;
using UtilityServices;
using RemaSoftware.Models.OrderViewModel;
using RemaSoftware.Models.PDFViewModel;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Constants;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAPIContabilitàInCloudService _apiContabilitàInCloudService;
        private readonly INotyfService _notyfService;
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;
        private readonly IImageService _imageService;
        private readonly PdfHelper _pdfHelper;
        private readonly IConfiguration _configuration;
        private readonly OrderHelper _orderHelper;
        private readonly string _basePathForImages;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IClientService clientService, IOperationService operationService,
            PdfHelper pdfHelper, IImageService imageService, IAPIContabilitàInCloudService apiContabilitàInCloudService,
            INotyfService notyfService, IConfiguration configuration, OrderHelper orderHelper)
        {
            _orderService = orderService;
            _clientService = clientService;
            _operationService = operationService;
            _pdfHelper = pdfHelper;
            _imageService = imageService;
            _apiContabilitàInCloudService = apiContabilitàInCloudService;
            _notyfService = notyfService;
            _configuration = configuration;
            _orderHelper = orderHelper;
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
            vm.RedirectUrlAfterCreation = Url.Action("OrderSummary", "Order");

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
                Operations = availableOperations?.Select(s=>new SelectListItem
                {
                    Text = s.Name,
                    Value = $"{s.OperationID}-{s.Name}"
                }).ToList()
            };
            vm.OldOrders_SKU.Insert(0, "");

            vm.RedirectUrlAfterCreation = Url.Action("Index", "Home");
            return View(vm);
        }

        [HttpPost]
        public JsonResult NewOrder(NewOrderViewModel model)
        {
            var validationResult = this.ValidateNewOrderViewModelAndSetDataOut(model);
            if (validationResult != "")
                return new JsonResult(new {Result = false, ToastMessage = validationResult});

            if (!string.IsNullOrEmpty(model.Photo))
            {
                var iamgeName = _imageService.SavingOrderImage(model.Photo, _basePathForImages);
                model.Order.Image_URL = iamgeName;
            }

            model.Order.Number_Pieces_InStock = model.Order.Number_Piece;
            model.Order.DataIn = DateTime.UtcNow;
            model.Order.Price_Uni = model.uni_price ?? 0;
            model.Order.Status = OrderStatusConstants.STATUS_ARRIVED;

            // aggiungo sul db le operazioni nuove
            var operationsSelectedToCreate = model.OperationsSelected.Where(w=>w.StartsWith("0")).Select(s=>s.Split('-').Last());
            var addedOps =_operationService.AddOperations(
                operationsSelectedToCreate.Select(s=>new Operation
                {
                    Name = s, Description = s
                }).ToList());
            
            // aggiungo all'ordine le operazioni selezionate
            var operationsSelectedAlredyExisting = model.OperationsSelected.Where(w=>!w.StartsWith("0")).Select(s=>int.Parse(s.Split('-').First())).ToList();
            model.Order.Order_Operation = operationsSelectedAlredyExisting.Union(addedOps.Select(s=>s.OperationID)).Select((s, index) => new Order_Operation
            {
                Ordering = index + 1, //+1 perchè parte da 0...
                OperationID = s
            }).ToList();
            
            try
            {
                var addedOrder = _orderHelper.AddOrderAndSendToFattureInCloud(model.Order);
                return new JsonResult(new { Result = true, OrderId = addedOrder.OrderID });
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante la creazione dell'ordine.");
                return new JsonResult(new { Result = false, ToastMessage = "Errore durante la creazione dell'ordine." });
            }
        }

        [HttpGet]
        public IActionResult GetEditOrderOperationsModal(int orderId)
        {
            var vm = new EditOrderOperationsViewModel();
            
            var order = _orderService.GetOrderWithOperationsById(orderId);
            if (order == null)
            {
                Logger.Error($"GetOperationsForEdit - order not found for id: {orderId}");
                return new JsonResult(new { Result = false, ToastMessage="Ordine non trovato."});
            }
            vm.OrderId = orderId;
            
            var allAvailableOperations = _operationService.GetAllOperations();
            vm.Operations = allAvailableOperations?.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = $"{s.OperationID}-{s.Name}"
            }).ToList();
            
            vm.OperationsSelected = order.Order_Operation.Select(s => $"{s.OperationID}-{s.Operations.Name}").ToList();
            
            return PartialView("_EditOrderOperationsModal", vm);
        }
        
        public JsonResult GetOperationsByOrderId(int orderId)
        {
            var ops = _orderService.GetOperationsByOrderId(orderId)
                .Select(s=>$"{s.OperationID}-{s.Name}").ToArray();
            return new JsonResult(new { Result = ops});
        }

        public JsonResult SaveDuplicateOrder(CopyOrderViewModel model)
        {
            var validationResult = this.ValidateDuplicateOrderViewModel(model);
            if (validationResult != "")
                return new JsonResult(new {Result = false, ToastMessage = validationResult});
            
            var oldOrder = _orderService.GetOrderWithOperationsById(model.OrderId);
            Order newOrder = new Order();
            newOrder.DDT = model.Code_DDT;
            newOrder.Number_Piece = model.NumberPiece;
            newOrder.Number_Pieces_InStock = model.NumberPiece;
            newOrder.DataIn = DateTime.UtcNow;
            newOrder.ClientID = oldOrder.ClientID;
            newOrder.DataOut = oldOrder.DataOut;
            newOrder.Description = oldOrder.Description;
            newOrder.Image_URL = oldOrder.Image_URL;
            newOrder.Name = oldOrder.Name;
            newOrder.Note = oldOrder.Note;
            newOrder.SKU = oldOrder.SKU;
            newOrder.Price_Uni = oldOrder.Price_Uni;
            newOrder.Status = Constants.OrderStatusConstants.STATUS_ARRIVED;

            // aggiungo all'ordine le operazioni selezionate
            newOrder.Order_Operation = oldOrder.Order_Operation.Select(s => new Order_Operation
            {
                OperationID = s.OperationID,
                Ordering = s.Ordering
            }).ToList();

            try
            {
                var addedOrder = _orderHelper.AddOrderAndSendToFattureInCloud(newOrder);
                return new JsonResult(new { Result = true, OrderId = addedOrder.OrderID, ToastMessage = "Ordine duplicato con successo" });
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante la duplicazione dell'ordine: {model.OrderId}.");
                return new JsonResult(new { Result = false, ToastMessage = "Errore durante la duplicazione dell'ordine." });
            }

        }
        
        public JsonResult EditOrderOperations(EditOrderOperationsViewModel model)
        {
            try
            {
                var operationsFromUser = model.OperationsSelected.Select((s, index) => new Operation
                {
                    OperationID = int.Parse(s.Split("-").First()),
                    Name = s.Split("-").Last()
                }).ToList();
                
                var newOperationsToCreate = operationsFromUser.Where(w => w.OperationID == 0).ToList();
                
                var addedOps = _operationService.AddOperations(
                    newOperationsToCreate
                );
                
                foreach (var opAdded in addedOps)
                {
                    var aa = operationsFromUser.Single(s => s.Name == opAdded.Name);
                    aa.OperationID = opAdded.OperationID;
                }
                
                // rimuovo tutte le operazioni per quell'ordine e le riaggiungo
                _operationService.RemoveAllOrderOperations(model.OrderId);
                _orderService.AddOrderOperation(model.OrderId, operationsFromUser.Select(s=>s.OperationID).ToList());
                
                return new JsonResult(new { Result = true, ToastMessage="Operazioni modificate correttamente."});
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

        [HttpGet]
        public IActionResult OrdersNotExtinguished()
        {
            var vm = _orderService.GetOrdersNotCompleted(); 
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string currentStatus, int outgoing_orders)
        {
            try
            {
                _orderService.UpdateOrderStatus(orderId, outgoing_orders);
                _notyfService.Success("Numero pezzi modificato correttamente");
            }
            catch(Exception e)
            {
                _notyfService.Error(e.Message);
            }

            return RedirectToAction("OrdersNotExtinguished");
        }

        public JsonResult DeleteProduct(int productId)
        {
            try
            {
                _apiContabilitàInCloudService.DeleteOrder(productId.ToString());
            }
            catch(Exception e)
            {
                Logger.Error(e, $"Problema di connessione con ContabilitàInCloud");
                return new JsonResult(new { Error = e, ToastMessage = $"Problema di connessione con ContabilitàInCloud. Contattare gli sviluppatori" });
            }
            try
            {
                var deleteResult = _orderService.DeleteOrderByID(productId);
                return new JsonResult(new { Result = deleteResult, ToastMessage = "Articolo di magazzino eliminato correttamente." });
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error deleting stockArticle: {productId}");
                return new JsonResult(new { Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'articolo di magazzino." });
            }
        }

        #region Models validation

        private string ValidateNewOrderViewModelAndSetDataOut(NewOrderViewModel model)
        {
            DateTime parsedDateTime;
            DateTime.TryParseExact(model.DataOutStr, "dd/MM/yyyy", null, DateTimeStyles.None, out parsedDateTime);
            model.Order.DataOut = parsedDateTime;
            
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

        private string ValidateDuplicateOrderViewModel(CopyOrderViewModel model)
        {
            if (string.IsNullOrEmpty(model.Code_DDT))
                return "Codice DDT mancante.";
            return "";
        }

        #endregion
    }
}