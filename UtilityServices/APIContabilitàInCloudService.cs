using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UtilityServices.Dtos;

namespace UtilityServices
{
    public class APIContabilitàInCloudService : IAPIContabilitàInCloudService
    {
        private readonly IConfiguration _configuration;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public APIContabilitàInCloudService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddOrderCloud(OrderDto order)
        {
            Logger.Info("Creazione prodotto su ContabilitàInCloud");

            var apiEndpoint = _configuration["ApiContabilitàInCloud:RevisoApiUrl"] + _configuration["ApiContabilitàInCloud:ApiEndpointProducts"];
            var apiSecretToken = _configuration["ApiContabilitàInCloud:X-AppSecretToken"];
            var apiAgreementToken = _configuration["ApiContabilitàInCloud:X-AgreementGrantToken"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Content-Type", _configuration["ApiContabilitàInCloud:Content-Type"]);
            httpWebRequest.Headers.Add("X-AppSecretToken", apiSecretToken);
            httpWebRequest.Headers.Add("X-AgreementGrantToken", apiAgreementToken);
#if DEBUG

#else
            var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
            httpWebRequest.Proxy = myProxy;
#endif
            OrderAPIContabilità orderToSendInCloud = new OrderAPIContabilità()
            {
                productNumber = order.Id.ToString(),
                name = order.SKU,
                description = order.DDT,
                salesPrice = order.Price_Uni,
                unit = new UnitNumberContabilità()
                {
                    unitNumber = 1
                },
                productGroup = new ProductGroupContabilità()
                {
                    productGroupNumber = 3
                }
            };

            string stringjson = JsonConvert.SerializeObject(orderToSendInCloud);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(stringjson);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Logger.Info($"Creazione - Risposta da ContabilitàInCloud: {result}");
        }

        public void DeleteOrder(string productId)
        {
            Logger.Info("Eliminazione prodotto su ContabilitàInCloud");

            var apiEndpoint = _configuration["ApiContabilitàInCloud:RevisoApiUrl"] + _configuration["ApiContabilitàInCloud:ApiEndpointProducts"] + productId;
            var apiSecretToken = _configuration["ApiContabilitàInCloud:X-AppSecretToken"];
            var apiAgreementToken = _configuration["ApiContabilitàInCloud:X-AgreementGrantToken"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.Method = "DELETE";
            httpWebRequest.Headers.Add("Content-Type", _configuration["ApiContabilitàInCloud:Content-Type"]);
            httpWebRequest.Headers.Add("X-AppSecretToken", apiSecretToken);
            httpWebRequest.Headers.Add("X-AgreementGrantToken", apiAgreementToken);
#if DEBUG

#else
            var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
            httpWebRequest.Proxy = myProxy;
#endif

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Logger.Info($"Eliminazione #{productId} - Risposta da ContabilitàInCloud: {result}");
        }

        public void UpdateInventory(OrderDto order)
        {
            Logger.Info("Aggiorno inventario su ContabilitàInCloud");

            var apiEndpoint = _configuration["ApiContabilitàInCloud:RevisoApiUrl"] + _configuration["ApiContabilitàInCloud:ApiEndpointPurchaseDDT"];
            var apiSecretToken = _configuration["ApiContabilitàInCloud:X-AppSecretToken"];
            var apiAgreementToken = _configuration["ApiContabilitàInCloud:X-AgreementGrantToken"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Content-Type", _configuration["ApiContabilitàInCloud:Content-Type"]);
            httpWebRequest.Headers.Add("X-AppSecretToken", apiSecretToken);
            httpWebRequest.Headers.Add("X-AgreementGrantToken", apiAgreementToken);
#if DEBUG

#else
            var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
            httpWebRequest.Proxy = myProxy;
#endif
            ProductDetails productDetails = new ProductDetails()
            {
                productLines = new List<ProductLines>()
            };
            productDetails.productLines.Add(new ProductLines()
            {
                product = new Product()
                {
                    id = order.Id.ToString(),
                    name = order.Name
                },
                lineNr = 1,
                quantity = order.Number_Piece,
                vatInfo = new VatInfo()
                {
                    vatAccount = new VatAccount()
                },
                unit = new Unit()
            });

            DDTPurchase ddtToSendInCloud = new DDTPurchase()
            {
                ownerDeliveryNoteReference = order.DDT,
                ownerDeliveryNoteReferenceDate = order.DataIn.ToString("yyyy-MM-dd"),
                owner = new Owner()
                {
                    vatZone = new VatZone() { }
                },
                numberSeries = new NumberSeries()
                {
                    numberSeriesSequenceElement = new NumberSeriesSequenceElement() { }
                },
                productDetails = productDetails,
                date = order.DataIn.ToString("yyyy-MM-dd"),
                additionalInfo = new AdditionalInfo() { },
                pdf = new Pdf()
                {
                    download = "https://reviso-delivery-note-app.azurewebsites.net/api/delivery-notes/purchase/print/" + order.Id
                }

            };

            string stringjson = JsonConvert.SerializeObject(ddtToSendInCloud);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(stringjson);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Logger.Info($"Update giacenze #{order.Id} - Risposta da ContabilitàInCloud: {result}");
        }

        private class OrderAPIContabilità
        {
            public string productNumber { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public decimal salesPrice { get; set; }
            public UnitNumberContabilità unit { get; set; }
            public ProductGroupContabilità productGroup { get; set; }

        }

        private class UnitNumberContabilità
        {
            public int unitNumber { get; set; }
        }

        private class ProductGroupContabilità
        {
            public int productGroupNumber { get; set; }
        }

        public class VatZone
        {
            public int vatZoneNumber { get; set; } = 1;
            public int id { get; set; } = 1;

        }
        public class Owner
        {
            public VatZone vatZone { get; set; }
            public int id { get; set; } = 1;

        }
        public class NumberSeriesSequenceElement
        {
            public int number { get; set; } = 1;
            public int id { get; set; } = 240;

        }
        public class NumberSeries
        {
            public string prefix { get; set; } = "DDTA";
            public string sequenceType { get; set; } = "Continious";
            public NumberSeriesSequenceElement numberSeriesSequenceElement { get; set; }
            public int id { get; set; } = 30;

        }
        public class Product
        {
            public string name { get; set; }
            public string id { get; set; }

        }
        public class VatAccount
        {
            public string id { get; set; } = "V022";

        }
        public class VatInfo
        {
            public VatAccount vatAccount { get; set; }
            public decimal vatRate { get; set; } = 22;

        }
        public class Unit
        {
            public string name { get; set; } = "pezzi";
            public int id { get; set; } = 1;

        }
        public class ProductLines
        {
            public Product product { get; set; }
            public int lineNr { get; set; }
            public VatInfo vatInfo { get; set; }
            public int quantity { get; set; }
            public Unit unit { get; set; }
            public decimal unitNetPrice { get; set; } = 0;
            public decimal totalNetAmount { get; set; } = 0;
            public decimal totalGrossAmount { get; set; } = 0;
            public decimal discountPercentage { get; set; } = 0;
            public decimal totalVatAmount { get; set; } = 0;
            public bool manuallyEditedSalesPrice { get; set; } = false;

        }
        public class ProductDetails
        {
            public IList<ProductLines> productLines { get; set; }

        }
        public class AdditionalInfo
        {
            public string currency { get; set; } = "EUR";
            public int exchangeRate { get; set; } = 100;

        }
        public class Pdf
        {
            public string download { get; set; } 

        }
        public class DDTPurchase
        {
            public string ownerDeliveryNoteReference { get; set; }
            public string ownerDeliveryNoteReferenceDate { get; set; }
            public bool affectsInStockCounter { get; set; } = true;
            public string deliveryNoteType { get; set; } = "Purchase";
            public string deliveryNoteStatus { get; set; } = "Issued";
            public Owner owner { get; set; }
            public NumberSeries numberSeries { get; set; }
            public ProductDetails productDetails { get; set; }
            public string date { get; set; }
            public AdditionalInfo additionalInfo { get; set; }
            public Pdf pdf { get; set; }

        }
    }
}
