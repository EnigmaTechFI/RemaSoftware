using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RemaSoftware.ContextModels;


namespace RemaSoftware.DALServices.Impl
{
    public class APIFatturaInCloudService : IAPIFatturaInCloudService
    {

        private const string URL = "https://api.fattureincloud.it/v1/prodotti/nuovo";
        private const string ApiUID = "843721";
        private const string ApiKEY = "c66e80a04e611edbcdef1bfe00833d58";

        public bool AddOrderCloud(Order order)
        {
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

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(stringjson);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            return true; 
        }
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
