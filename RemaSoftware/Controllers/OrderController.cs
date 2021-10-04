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
        private readonly IPdfService _pdfService;
        private readonly IClientService _clientService;
        private readonly IOperationService _operationService;
        private readonly PdfHelper _pdfHelper;
        private const string URL = "https://api.fattureincloud.it/v1/prodotti/nuovo";
        private const string ApiUID = "843721";
        private const string ApiKEY = "c66e80a04e611edbcdef1bfe00833d58";
        private IWebHostEnvironment _hostEnvironment;


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OrderController(IOrderService orderService, IPdfService pdfService, IClientService clientService, IOperationService operationService, PdfHelper pdfHelper, IWebHostEnvironment hostEnvironment)
        {
            _orderService = orderService;
            _pdfService = pdfService;
            _clientService = clientService;
            _operationService = operationService;
            _pdfHelper = pdfHelper;
            _hostEnvironment = hostEnvironment;
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
            var webrootpath = _hostEnvironment.WebRootPath;

            string file_path = Directory.GetParent(webrootpath).Parent.FullName + "/ImmaginiOrdini/" + guid + ".png";

            System.IO.File.WriteAllBytes(file_path, data);

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

                //API FatturaInCloud
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                OrderAPI test = new OrderAPI()
                {
                    api_uid = ApiUID,
                    api_key = ApiKEY,
                    cod = order.SKU,
                    nome = order.Name,
                    desc = order.Description,
                    prezzo_ivato = order.Price_Tot,
                    prezzo_netto = 0,
                    prezzo_lordo = 0,
                    costo = order.Price_Tot,
                    cod_iva = 0,
                    um = "",
                    categoria = "",
                    note = "",
                    magazzino = true,
                    giacenza_iniziale = order.Number_Piece
                };

                string stringjson = JsonConvert.SerializeObject(test);
                Console.WriteLine(stringjson);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(stringjson);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Do Something
            }

            //TODO API, PDF
            

            

            return RedirectToAction("Index", "Home");
        }

        class OrderAPI
        {
            public string api_uid { get; set; }
            public string api_key { get; set; }
            public string cod { get; set; }
            public string nome { get; set; }
            public string desc { get; set; }
            public double prezzo_ivato { get; set; }
            public double prezzo_netto { get; set; }
            public double prezzo_lordo { get; set; }
            public double costo { get; set; }
            public int cod_iva { get; set; }
            public string um { get; set; }
            public string categoria { get; set; }
            public string note { get; set; }
            public bool magazzino { get; set; }
            public int giacenza_iniziale { get; set; }
            
        }
    }
}