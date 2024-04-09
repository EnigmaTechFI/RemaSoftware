using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.OrderViewModel;
using RemaSoftware.WebApp.Models.PDFViewModel;
using RemaSoftware.WebApp.Validation;

namespace RemaSoftware.WebApp.Controllers
{
    
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        
        private readonly INotyfService _notyfService;
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;
        private readonly IConfiguration _configuration;
        private readonly OrderHelper _orderHelper;
        private readonly PriceHelper _priceHelper;
        private readonly OrderValidation _orderValidation;
        private readonly IProductService _productService;
        private readonly ISubBatchService _subBatchService;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IClientService clientService, IOperationService operationService, PriceHelper priceHelper,
            INotyfService notyfService, IConfiguration configuration, OrderHelper orderHelper, OrderValidation orderValidation, IProductService productService, ISubBatchService subBatchService)
        {
            _orderService = orderService;
            _clientService = clientService;
            _operationService = operationService;
            _notyfService = notyfService;
            _configuration = configuration;
            _orderHelper = orderHelper;
            _priceHelper = priceHelper;
            _orderValidation = orderValidation;
            _productService = productService;
            _subBatchService = subBatchService;
        }

        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente +"," +Roles.COQ)]
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
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente +"," +Roles.COQ)]
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
                _notyfService.Error(e.Message);
                return RedirectToAction("QualityControl");
            }
            
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente+"," +Roles.COQ)]
        [HttpGet]
        public IActionResult QualityControl()
        {
            return View(_orderHelper.GetQualityControlViewModel());
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult SubBatchMonitoring(int id, string url = "")
        {
            return View(_orderHelper.GetSubBatchMonitoring(id, url));
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult OrderSummary(int subBatchId = 0)
        {
            return View(new OrderSummaryViewModel()
            {
                Ddt_In = _orderHelper.GetAllDdtInActive_NoPagination(),
                SubBatchId = subBatchId
            });
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult OrderSummaryEnded()
        {
            return View(new OrderSummaryViewModel()
            {
                Ddt_In = _orderHelper.GetAllDdtInEnded_NoPagination()
            });
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult DownloadPdfOrder(int id)
        {
            var subBatch = _subBatchService.GetSubBatchById(id);
            var vm = new PDFViewModel()
            {
                QRCode = _orderHelper.CreateQRCode(id),
                SubBatch = subBatch,
                BasePathImages = $"{_configuration["ApplicationUrl"]}{_configuration["ImagesEndpoint"]}order/"
            };
            return View("../Pdf/SingleOrderSummary", vm);
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult EditOrder(int id)
        {
            var ddt = _orderHelper.GetDdtInById(id);
            var vm = new NewOrderViewModel
            {
                Price = ddt.Price_Uni.ToString("0.00").Replace(".", ","),
                Ddt_In = ddt,
            };
            return View(vm);
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpPost]
        public IActionResult EditOrder(NewOrderViewModel model)
        {
            try {
                var validationResult = _orderValidation.ValidateEditOrderViewModelAndSetDefaultData(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                    return View(NewOrderViewModelThrow(model));
                }
                var result = _orderHelper.EditDdtIn(model);
                _notyfService.Success("Commessa aggiornata correttamente");
                return RedirectToAction("OrderSummary", "Order", new {subBatchId = result.SubBatchID});
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante l&#39;aggiornamento dell&#39;ordine.");
                return View(NewOrderViewModelThrow(model));
            }
        }

        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult NewOrder(int productId)
        {
            var products = productId == 0 ? _productService.GetAllProducts() : new List<Product>();
            products = products.OrderBy(t => t.SKU).ToList();
            var prices = _priceHelper.GetPrices(productId);
            
            var vm = new NewOrderViewModel
            {
                Clients = _clientService.GetAllClients(),
                Operations = _operationService.GetAllOperationsWithOutCOQAndEXTRA()?.Select(s=>new SelectListItem
                {
                    Text = s.Name,
                    Value = $"{s.OperationID}-{s.Name}"
                }).OrderBy(y => y.Text).ToList(),
                Ddt_In = new Ddt_In()
                {
                    ProductID = productId,
                },
                Products = products,
                Prices = prices
            };
            return View(vm);
        }
        private NewOrderViewModel NewOrderViewModelThrow(NewOrderViewModel model)
        {
            var op = _operationService.GetAllOperationsWithOutCOQAndEXTRA();
            model.Operations = op?.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = $"{s.OperationID}-{s.Name}"
            }).ToList();
            if (model.Ddt_In.ProductID != 0)
            {
                model.Products = new List<Product>();
            }
            else
            {
                model.Products = _productService.GetAllProducts();
            }

            return model;
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpPost]
        public IActionResult NewOrder(NewOrderViewModel model)
        {
            try
            {
                var validationResult = _orderValidation.ValidateNewOrderViewModelAndSetDefaultData(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                    return View(NewOrderViewModelThrow(model));
                }
                var result = _orderHelper.AddNewDdtIn(model);
                _notyfService.Success("Commessa registrata correttamente");
                return RedirectToAction("OrderSummary", "Order", new{subBatchId = result.SubBatchID});
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante la creazione dell'ordine.");
                return View(NewOrderViewModelThrow(model));
            }
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult BatchInStock()
        {
            return View(new BatchInStockViewModel()
            {
                SubBatches = _orderHelper.GetSubBatchesStatus(OrderStatusConstants.STATUS_ARRIVED)
            });
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public async Task<IActionResult> EmitDDT(int id)
        {
            try
            {
                return RedirectToAction("BatchInDelivery", new {pdfUrl = await _orderHelper.EmitDDT(id)}); 
            }
            catch (Exception e)
            {
                _notyfService.Error(e.Message);
                return RedirectToAction("BatchInDelivery");
            }
            
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult DDTEmitted()
        {
            return View(_orderHelper.GetDDTEmittedViewModel());
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult BatchInDelivery(string pdfUrl = "empty")
        {
            return View(new BatchInDeliveryViewModel()
            {
                pdfUrl = pdfUrl,
                DdtOuts = _orderHelper.GetDdtsInDelivery()
            });
        }
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
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

        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult BatchInProduction()
        {
            return View(new BatchInProductionViewModel()
            {
                SubBatches = _orderHelper.GetSubBatchesStatus(OrderStatusConstants.STATUS_WORKING)
            });
        }
        
        public JsonResult DeleteProduct(int productId)
        {
            try
            {
                return new JsonResult(new { Error = "", Data = _orderHelper.DeleteOrder(productId), ToastMessage = $"Ordine eliminato correttamente" });
            }
            catch(Exception e)
            {
                Logger.Error(e, $"Problema di connessione con FattureInCloud");
                return new JsonResult(new { Error = e.Message, Data = 0, ToastMessage = $"Problema di connessione con FattureInCloud.Contattare gli sviluppatori" });
            }
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult PartialDDT(int id, int clientId)
        {
            try
            {
                return View(_orderHelper.GetPartialDDTViewModel(id, clientId));
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                _notyfService.Error("Errore durante il recupero delle ddt.");
                return RedirectToAction("BatchInDelivery");
            }
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpPost]
        public async Task<IActionResult> PartialDDT(PartialDDTViewModel model)
        {
            try
            {
                return RedirectToAction("BatchInDelivery", new {pdfUrl = await _orderHelper.EmitPartialDdtIn(model)});
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                _notyfService.Error(e.Message);
                return RedirectToAction("PartialDDT", new {id = model.DdtId, clientId = model.ClientId});
            }
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente+"," + Roles.Magazzino)]
        [HttpGet]
        public IActionResult StockSummary()
        {
            try
            {
                return View(_orderHelper.GetStockSummaryViewModel());
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                return RedirectToAction("BatchInStock", "Order");
            }
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public JsonResult StockVariation(int id, int pieces)
        {
            try
            {
                _orderHelper.StockVariation(id, pieces);
                return new JsonResult(new { Result = true,  Message = "Variazione inventario effettuata correttamente." });
            }
            catch (Exception e)
            {
                return new JsonResult(new { Result = false, Message=e.Message});
            }
        }
        
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public JsonResult DeleteDdtAssociation(int id)
        {
            try
            {
                _orderHelper.DeleteDdtAssociation(id);
                return new JsonResult(new { Result = true, Data = id,  Message = "DDT di uscita annullata correttamente." });
            }
            catch (Exception e)
            {
                return new JsonResult(new { Result = false, Data = 0, Message=e.Message});
            }
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult BatchToSupplier()
        {
            try
            {
                return View(_orderHelper.GetBatchToSupplierViewModel());
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                return RedirectToAction("Index", "Home");
            }
            
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult ReloadSubBatchFromSupplier(int id)
        {
            try
            {
                return View(_orderHelper.GetReloadSubBatchFromSupplierViewModel(id));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                return RedirectToAction("Index", "Home");
            }
            
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpPost]
        public IActionResult ReloadSubBatchFromSupplier(ReloadSubBatchFromSupplierViewModel model)
        {
            try
            {
                _orderHelper.ReloadSubBatchFromSupplier(model);
                return RedirectToAction("BatchToSupplier");
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error(e.Message);
                return RedirectToAction("ReloadSubBatchFromSupplier", new{id = model.DDTSupplierId});
            }
            
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult ExitToSupplier(int id)
        {
            try
            {
                return View(_orderHelper.GetExitToSupplierViewModel(id));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                return RedirectToAction("SubBatchMonitoring", new { id = id });
            }
            
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpPost]
        public IActionResult ExitToSupplier(ExitToSupplierViewModel model)
        {
            try
            {
                var validationResult = _orderValidation.ValidateDDTSupplier(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                    return RedirectToAction("ExitToSupplier", new{id =model.SubBatch.SubBatchID});
                }
                var result = _orderHelper.RegisterExitSubBatch(model);
                return RedirectToAction("SubBatchMonitoring", new { id = model.SubBatch.SubBatchID, url = result});
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error(e.Message);
                return RedirectToAction("ExitToSupplier", new { id = model.SubBatch.SubBatchID });
            }
            
        }
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpGet]
        public IActionResult DuplicateOrder(int id)
        {
            try
            {
                return View(_orderHelper.GetDuplicateOrderViewModelForDuplicate(id));
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error(e.Message);
                return RedirectToAction("OrderSummary");
            }
        }
        
        
        [Authorize(Roles = Roles.Admin +"," + Roles.Dipendente)]
        [HttpPost]
        public IActionResult DuplicateOrder(DuplicateOrderViewModel model)
        {
            try
            {
                var validationResult = _orderValidation.ValidateDuplicateOrderViewModelAndSetDefaultData(model);
                if (validationResult != "")
                {
                    _notyfService.Error(validationResult);
                    return View(model);
                }
                var result = _orderHelper.AddDuplicateDdtIn(model);
                _notyfService.Success("Commessa registrata correttamente");
                return RedirectToAction("OrderSummary", "Order", new{subBatchId = result.SubBatchID});
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                _notyfService.Error("Errore durante la creazione dell'ordine.");
                return View(model);
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