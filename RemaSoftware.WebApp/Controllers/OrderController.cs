using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.OrderViewModel;
using RemaSoftware.WebApp.Models.PDFViewModel;
using RemaSoftware.WebApp.Validation;

namespace RemaSoftware.WebApp.Controllers
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
        private readonly IConfiguration _configuration;
        private readonly OrderHelper _orderHelper;
        private readonly OrderValidation _orderValidation;
        private readonly IProductService _productService;
        private readonly string _basePathForImages;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IClientService clientService, IOperationService operationService,
            IImageService imageService, IAPIFatturaInCloudService apiFatturaInCloudService,
            INotyfService notyfService, IConfiguration configuration, OrderHelper orderHelper, OrderValidation orderValidation, IProductService productService)
        {
            _orderService = orderService;
            _clientService = clientService;
            _operationService = operationService;
            _imageService = imageService;
            _apiFatturaInCloud = apiFatturaInCloudService;
            _notyfService = notyfService;
            _configuration = configuration;
            _orderHelper = orderHelper;
            _basePathForImages =
                (Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName + _configuration["ImagePath"]).Replace("/", "\\");
            _orderValidation = orderValidation;
            _productService = productService;
        }

        [HttpPost]
        public JsonResult EndOrder([FromBody] SubBatchToEndDto dto)
        {
            try
            {
                return new JsonResult(new { Result = true, Data = _orderHelper.EndOrder(dto), ToastMessage="Ordine concluso correttamente."});
            }
            catch (Exception e)
            {
                return new JsonResult(new { Result = false, Data = 0, ToastMessage=e.Message});
            }
        }

        [HttpPost]
        public IActionResult SubBatchAtControl(QualityControlViewModel model)
        {
            try
            {
                _orderHelper.RegisterBatchAtCOQ(model.subBatchId);
                return RedirectToAction("QualityControl");
            }
            catch (Exception e)
            {
                _notyfService.Error("Errore durante la registrazione del sotto lotto.");
                return RedirectToAction("QualityControl");
            }
            
        }
        
        [HttpGet]
        public IActionResult QualityControl()
        {
            return View(_orderHelper.GetQualityControlViewModel());
        }

        [HttpGet]
        public IActionResult SubBatchMonitoring(int id)
        {
            return View(_orderHelper.GetSubBatchMonitoring(id));
        }

        [HttpGet]
        public IActionResult OrderSummary()
        {
            return View(new OrderSummaryViewModel()
            {
                Ddt_In = _orderHelper.GetAllDdtInActive_NoPagination()
            });
        }
        
        [HttpGet]
        public IActionResult OrderSummaryEnded()
        {
            return View(new OrderSummaryViewModel()
            {
                Ddt_In = _orderHelper.GetAllDdtInEnded_NoPagination()
            });
        }

        [HttpGet]
        public JsonResult OrderSummaryCompleted()
        {
            var orders = _orderService.GetOrdersCompleted();
            return new JsonResult(new { Orders = orders });
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
        public IActionResult EditOrder(int id)
        {
            var vm = new NewOrderViewModel
            {
                Clients = _clientService.GetAllClients(),
                Operations = _operationService.GetAllOperationsWithOutCOQ()?.Select(s=>new SelectListItem
                {
                    Text = s.Name,
                    Value = $"{s.OperationID}-{s.Name}"
                }).ToList(),
                Ddt_In = _orderHelper.GetDdtInById(id)
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult NewOrder(int productId)
        {
            var products = productId == 0 ? _productService.GetAllProducts() : new List<Product>();
            var vm = new NewOrderViewModel
            {
                Clients = _clientService.GetAllClients(),
                Operations = _operationService.GetAllOperationsWithOutCOQ()?.Select(s=>new SelectListItem
                {
                    Text = s.Name,
                    Value = $"{s.OperationID}-{s.Name}"
                }).ToList(),
                Ddt_In = new Ddt_In()
                {
                    ProductID = productId,
                },
                Products = products
            };
            return View(vm);
        }

        private NewOrderViewModel NewOrderViewModelThrow(NewOrderViewModel model)
        {
            model.Operations = _operationService.GetAllOperationsWithOutCOQ()?.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = $"{s.OperationID}-{s.Name}"
            }).ToList();
            return model;
        }

        [HttpPost]
        public IActionResult NewOrder(NewOrderViewModel model)
        {
            try {
                var validationResult = _orderValidation.ValidateNewOrderViewModelAndSetDefaultData(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                    return View(NewOrderViewModelThrow(model));
                }
                var result = _orderHelper.AddNewDdtIn(model);
                _notyfService.Success("Commessa registrata correttamente");
                return RedirectToAction("ProductList", "Product", new{subBatchId = result.SubBatchID});
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante la creazione dell'ordine.");
                return View(NewOrderViewModelThrow(model));
            }
        }

        [HttpGet]
        public IActionResult BatchInStock()
        {
            return View(new BatchInStockViewModel()
            {
                SubBatches = _orderHelper.GetSubBatchesStatus(OrderStatusConstants.STATUS_ARRIVED)
            });
        }
        
        [HttpGet]
        public IActionResult EmitDDT(int id)
        {
            try
            {
                return RedirectToAction("BatchInDelivery", new {pdfUrl = _orderHelper.EmitDDT(id)}); 
            }
            catch (Exception e)
            {
                return RedirectToAction("BatchInDelivery");
            }
            
        }

        [HttpGet]
        public IActionResult DDTEmitted()
        {
            return View(_orderHelper.GetDDTEmittedViewModel());
        }
        
        [HttpGet]
        public IActionResult BatchInDelivery(string pdfUrl = "empty")
        {
            return View(new BatchInDeliveryViewModel()
            {
                pdfUrl = pdfUrl,
                DdtOuts = _orderHelper.GetDdtsInDelivery()
            });
        }
        
        [HttpGet]
        public JsonResult DeleteDDT(int id)
        {
            try
            {
                return new JsonResult(new {Data = _orderHelper.DeleteDDT(id), Error = ""});
            }
            catch (Exception e)
            {
                return new JsonResult(new {Data = "", Error = e.Message});
            }
        }

        /*END VERSIONE 2.0*/

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
            
            var allAvailableOperations = _operationService.GetAllOperationsWithOutCOQ();
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
            Order newOrder = new Order()
            {
                DDT = model.Code_DDT,
                Number_Piece = model.NumberPiece,
                Number_Pieces_InStock = model.NumberPiece,
                DataIn = DateTime.UtcNow,
                ClientID = oldOrder.ClientID,
                DataOut = oldOrder.DataOut,
                Description = oldOrder.Description,
                Image_URL = oldOrder.Image_URL,
                Name = oldOrder.Name,
                Note = oldOrder.Note,
                SKU = oldOrder.SKU,
                Price_Uni = oldOrder.Price_Uni,
                Status = OrderStatusConstants.STATUS_ARRIVED
            };

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
        public IActionResult BatchInProduction()
        {
            return View(new BatchInProductionViewModel()
            {
                SubBatches = _orderHelper.GetSubBatchesStatus(OrderStatusConstants.STATUS_WORKING)
            });
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

            return BadRequest();
        }

        public JsonResult DeleteProduct(int productId)
        {
            try
            {
                var order = _orderService.GetOrderById(productId);
                var result = _apiFatturaInCloud.DeleteOrder(order.ID_FattureInCloud);
            }
            catch(Exception e)
            {
                Logger.Error(e, $"Problema di connessione con FattureInCloud");
                return new JsonResult(new { Error = e, ToastMessage = $"Problema di connessione con FattureInCloud.Contattare gli sviluppatori" });
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

        private string ValidateDuplicateOrderViewModel(CopyOrderViewModel model)
        {
            if (string.IsNullOrEmpty(model.Code_DDT))
                return "Codice DDT mancante.";
            return "";
        }

        #endregion
    }
}