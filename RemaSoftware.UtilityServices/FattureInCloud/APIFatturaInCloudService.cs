using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using RemaSoftware.UtilityServices.Dtos;
using UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices.FattureInCloud
{
    public class APIFatturaInCloudService : IAPIFatturaInCloudService
    {
        private readonly IConfiguration _configuration;
        private readonly IFicBaseHttp _ficBaseHttp;
        private readonly string _ficAccessToken;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public APIFatturaInCloudService(IConfiguration configuration, string env, IFicBaseHttp ficBaseHttp)
        {
            _configuration = configuration;
            _ficBaseHttp = ficBaseHttp;
            _ficAccessToken = _configuration["ApiFattureInCloud:AccessToken"];
        }

        public string AddOrderCloud(OrderDto order)
        {
            Logger.Info("Inizio invio a ApiFattureInCloud");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FicApiUrls.ProductUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);
            #if !DEBUG
                var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
                httpWebRequest.Proxy = myProxy;
            #endif
            
            OrderAPI orderToSendInCloud = new OrderAPI()
            {
                code = order.DDT,
                name = order.SKU,
                description = order.Description,
                prezzo_ivato = false,
                net_price = order.Price_Uni.ToString("0.##"),
                gross_price = "0",
                net_cost = "0",
                
                measure = "",
                category = "",
                notes = "",
                in_stock = true,
                stock_initial = order.Number_Piece
            };

            string stringjson = JsonConvert.SerializeObject(new { data=orderToSendInCloud });

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
            return obj.data.id;
        }

        public string DeleteOrder(string productId)
        {
            Logger.Info("Inizio eliminazione da ApiFattureInCloud");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FicApiUrls.ProductUrl + productId);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";
            httpWebRequest.Headers.Add("authorization", "Bearer " + "/" + _ficAccessToken);
#if !DEBUG
                var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
                httpWebRequest.Proxy = myProxy;
#endif
            
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

        public bool UpdateOrderCloud(OrderToUpdateDto order)
        {
            Logger.Info("Inizio aggiornamento prodotto su ApiFattureInCloud");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FicApiUrls.ProductUrl + "/" + order.ID_FattureInCloud);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);
#if !DEBUG
                var myProxy = new WebProxy("http://winproxy.server.lan:3128/", true);
                httpWebRequest.Proxy = myProxy;
#endif

            OrderAPI orderToSendInCloud = new OrderAPI()
            {
                id = order.Id,
                code = order.DDT,
                name = order.SKU,
                description = order.Description,
                prezzo_ivato = false,
                net_price = order.Price_Uni.ToString("0.##"),
                gross_price = "0",
                net_cost = "0",
                cod_iva = 0,
                measure = "",
                category = "",
                notes = "",
                in_stock = true,
                stock_initial = order.Number_Piece
            };

            string stringjson = JsonConvert.SerializeObject(new {data=orderToSendInCloud});

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
            return true;
        }

        public async Task<int> AddClient(ClientDto client)
        {
            var result = await _ficBaseHttp.Post<ClientDtoResponseFic>(FicApiUrls.ClientUrl, new {data = client});
            return result.Data.FicId;
        }
    }

    class OrderAPI
    {
        public string id { get; set; }
        // public string api_uid { get; set; }
        // public string api_key { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool prezzo_ivato { get; set; }
        public string net_price { get; set; }
        public string gross_price { get; set; }
        public string net_cost { get; set; }
        public int cod_iva { get; set; }
        public string measure { get; set; }
        public string category { get; set; }
        public string notes { get; set; }
        public bool in_stock { get; set; }
        public int stock_initial { get; set; }

    }
    class OrderToDelete{
        public string api_uid { get; set; }
        public string api_key { get; set; }
        public string id { get; set; }
    }
}
