using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using UtilityServices.Dtos;


namespace UtilityServices
{
    public class APIFatturaInCloudService : IAPIFatturaInCloudService
    {
        private readonly IConfiguration _configuration;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public APIFatturaInCloudService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AddOrderCloud(OrderDto order)
        {
            Logger.Info("Inizio invio a ApiFattureInCloud");

            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointNewProduct"];
            var apiUID = _configuration["ApiFattureInCloud:ApiUID"];
            var apiKey = _configuration["ApiFattureInCloud:ApiKEY"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
            httpWebRequest.Proxy = myProxy;

            OrderAPI orderToSendInCloud = new OrderAPI()
            {
                api_uid = apiUID,
                api_key = apiKey,
                cod = order.DDT,
                nome = order.SKU,
                desc = order.Description,
                prezzo_ivato = false,
                prezzo_netto = order.Price_Uni.ToString("0.##"),
                prezzo_lordo = "0",
                costo = "0",
                cod_iva = 0,
                um = "",
                categoria = "",
                note = "",
                magazzino = true,
                giacenza_iniziale = order.Number_Piece
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

            Logger.Info($"Creazione - Risposta da FattureInCloud: {result}");
            var obj = JsonConvert.DeserializeObject<dynamic>(result);
            return obj.id;
        }

        public string DeleteOrder(string productId)
        {
            Logger.Info("Inizio eliminazione da ApiFattureInCloud");

            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointDeleteProduct"];
            var apiUID = _configuration["ApiFattureInCloud:ApiUID"];
            var apiKey = _configuration["ApiFattureInCloud:ApiKEY"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
            httpWebRequest.Proxy = myProxy;

            OrderToDelete orderToSendInCloud = new OrderToDelete()
            {
                api_uid = apiUID,
                api_key = apiKey,
                id = productId,
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

            Logger.Info($"Delete - Risposta da FattureInCloud: {result}");
            var obj = JsonConvert.DeserializeObject<dynamic>(result);
            return obj.error_code;
        }

        public string UpdateOrderCloud(OrderToUpdateDto order)
        {
            Logger.Info("Inizio aggiornamento prodotto su ApiFattureInCloud");

            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointUpdateProduct"];
            var apiUID = _configuration["ApiFattureInCloud:ApiUID"];
            var apiKey = _configuration["ApiFattureInCloud:ApiKEY"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
            httpWebRequest.Proxy = myProxy;

            OrderAPI orderToSendInCloud = new OrderAPI()
            {
                id = order.Id.ToString(),
                api_uid = apiUID,
                api_key = apiKey,
                cod = order.DDT,
                nome = order.SKU,
                desc = order.Description,
                prezzo_ivato = false,
                prezzo_netto = order.Price_Uni.ToString("0.##"),
                prezzo_lordo = "0",
                costo = "0",
                cod_iva = 0,
                um = "",
                categoria = "",
                note = "",
                magazzino = true,
                giacenza_iniziale = order.Number_Piece
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

            Logger.Info($"Creazione - Risposta da FattureInCloud: {result}");
            var obj = JsonConvert.DeserializeObject<dynamic>(result);
            return obj.success;
        }
    }

    class OrderAPI
    {
        public string id { get; set; }
        public string api_uid { get; set; }
        public string api_key { get; set; }
        public string cod { get; set; }
        public string nome { get; set; }
        public string desc { get; set; }
        public bool prezzo_ivato { get; set; }
        public string prezzo_netto { get; set; }
        public string prezzo_lordo { get; set; }
        public string costo { get; set; }
        public int cod_iva { get; set; }
        public string um { get; set; }
        public string categoria { get; set; }
        public string note { get; set; }
        public bool magazzino { get; set; }
        public int giacenza_iniziale { get; set; }

    }
    class OrderToDelete{
        public string api_uid { get; set; }
        public string api_key { get; set; }
        public string id { get; set; }
    }
}
