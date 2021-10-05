using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using RemaSoftware.DALServices;
using RemaSoftware.Helper;
using RemaSoftware.Models.ClientViewModel;
using UtilityServices;
using Microsoft.AspNetCore.Hosting;

namespace RemaSoftware.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAPIFatturaInCloudService _apiFatturaInCloud;
        private readonly IPdfService _pdfService;
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;
        private readonly IImageService _imageService;
        private readonly PdfHelper _pdfHelper;
        private IWebHostEnvironment _hostEnvironment;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IPdfService pdfService, IClientService clientService, IOperationService operationService, PdfHelper pdfHelper, IWebHostEnvironment hostEnvironment, IImageService imageService, IAPIFatturaInCloudService apiFatturaInCloudService)
        {
            _orderService = orderService;
            _pdfService = pdfService;
            _clientService = clientService;
            _operationService = operationService;
            _pdfHelper = pdfHelper;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _apiFatturaInCloud = apiFatturaInCloudService;
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
        public async Task<IActionResult> NewOrder(NewOrderViewModel model)
        {
            var webrootpath = _hostEnvironment.WebRootPath;

            string file_path = Directory.GetParent(webrootpath).Parent.FullName;

            var guid = _imageService.SavingOrderImage(model.Photo, file_path);

            model.Order.DataIn = DateTime.Now;

            model.Order.Image_URL = guid + ".png";

            model.Order.Price_Tot = model.Order.Price_Uni * model.Order.Number_Piece;

            var order_operationID = new List<int>();

            foreach (var id in model.Operation)
            {
                if (id.Flag == true)
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
                _apiFatturaInCloud.AddOrderCloud(order);

                //Schianta nella creazione del PDF

                /*try
                {
                    var vieString = await _pdfHelper.RenderViewToString("Pdf/SingleOrderSummary", order);
                    var fileBytes = _pdfService.GeneratePdf(vieString);

                    return File(fileBytes, "application/pdf");
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Errore durante la generazione del pdf.");
                }*/

                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Do Something
            }

            return RedirectToAction("Index", "Home");
        }

    }
}