﻿using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UtilityServices.Dtos;


namespace UtilityServices
{
    public class APIFatturaInCloudService : IAPIFatturaInCloudService
    {
        private readonly IConfiguration _configuration;

        public APIFatturaInCloudService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public bool AddOrderCloud(OrderDto order)
        {
            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpoint"];
            var apiUID = _configuration["ApiFattureInCloud:ApiUID"];
            var apiKey = _configuration["ApiFattureInCloud:ApiKEY"];
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            OrderAPI test = new OrderAPI()
            {
                api_uid = apiUID,
                api_key = apiKey,
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