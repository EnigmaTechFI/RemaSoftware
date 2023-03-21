using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using It.FattureInCloud.Sdk.Api;
using It.FattureInCloud.Sdk.Client;
using It.FattureInCloud.Sdk.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Dtos;
using RemaSoftware.UtilityServices.Interface;
using UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices.Implementation
{
    public class APIFatturaInCloudService : IAPIFatturaInCloudService
    {
        private readonly IConfiguration _configuration;
        private readonly string _ficAccessToken;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public APIFatturaInCloudService(IConfiguration configuration, string env)
        {
            _configuration = configuration;
            _ficAccessToken = _configuration["ApiFattureInCloud:AccessToken"];
        }

        public int AddClientCloud(Client client)
        {
            try
            {
                Logger.Info("Inizio creazione cliente su ApiFattureInCloud");
                var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointCompanies"];

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);
                ClientApi clientToSendInCloud = new ClientApi()
                {
                    type = "company",
                    name = client.Name,
                    addressCity = client.City,
                    addressStreet = client.Street + ", " + client.StreetNumber,
                    addressProvince = client.Province,
                    addressPostalCode = client.Cap,
                    taxCode = client.P_Iva,
                    vatNumber = client.P_Iva,
                };
                string stringjson = JsonConvert.SerializeObject(new { data = clientToSendInCloud });

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

                Logger.Info($"Creazione cliente - Risposta da FattureInCloud: {result}");
                var obj = JsonConvert.DeserializeObject<dynamic>(result);
                return obj.data.id;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                throw new Exception("Errore durante l'aggiunta del cliente a FattureInCloud.");
            }
        }

        public void UpdateClientCloud(Client client)
        {
            try
            {


                Logger.Info("Inizio aggiornamento cliente su ApiFattureInCloud");
                var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointCompanies"] + "/" + client.FC_ClientID;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);
                ClientApi clientToSendInCloud = new ClientApi()
                {
                    type = "company",
                    name = client.Name,
                    addressCity = client.City,
                    addressStreet = client.Street + ", " + client.StreetNumber,
                    addressProvince = client.Province,
                    addressPostalCode = client.Cap,
                    taxCode = client.P_Iva,
                    vatNumber = client.P_Iva,
                };
                string stringjson = JsonConvert.SerializeObject(new { data = clientToSendInCloud });

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

                Logger.Info($"Aggiornamento cliente - Risposta da FattureInCloud: {result}");
                var obj = JsonConvert.DeserializeObject<dynamic>(result);;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                throw new Exception("Errore durante l'aggiunta del cliente a FattureInCloud.");
            }
        }

        public (string, int) CreateDdtInCloud(Ddt_Out ddtOut)
        {
            Logger.Info("Inizio invio a ApiFattureInCloud");
            Configuration config = new Configuration();
            config.BasePath = _configuration["ApiFattureInCloud:BasePath"];
            config.AccessToken = _ficAccessToken;
            var apiInstance = new IssuedDocumentsApi(config);
            var companyId = _configuration["ApiFattureInCloud:CompanyId"];
            var products = new List<IssuedDocumentItemsListItem>();
            foreach (var item in ddtOut.Ddt_Associations)
            {
                products.Add(new IssuedDocumentItemsListItem()
                {
                    ProductId = Int32.Parse(item.Ddt_In.FC_Ddt_In_ID),
                    Qty = item.NumberPieces,
                    Name = item.Ddt_In.Product.Name,
                    Code = item.Ddt_In.Code,
                    Description = item.Ddt_In.Description,
                    Stock = true,
                    NetPrice = item.Ddt_In.Price_Uni,
                    Vat = new VatType()
                    {
                        Id = 0,
                        Value = 22
                    }
                    //TODO: Gestire iva nel caso la ddt sia una reso;
                });
            }
            var createIssuedDocumentRequest = new CreateIssuedDocumentRequest()
            {
                Data = new IssuedDocument()
                {
                    Type = IssuedDocumentType.DeliveryNote,
                    Entity = new Entity()
                    {
                        Name = ddtOut.Client.Name,
                        AddressCity = ddtOut.Client.City,
                        AddressProvince = ddtOut.Client.Province,
                        AddressStreet = ddtOut.Client.Street + ", " + ddtOut.Client.StreetNumber,
                        AddressPostalCode = ddtOut.Client.Cap,
                        VatNumber = ddtOut.Client.P_Iva,
                        TaxCode = ddtOut.Client.P_Iva
                    },
                    ItemsList = products,
                }
            };
            try
            {
                CreateIssuedDocumentResponse result = apiInstance.CreateIssuedDocument(Int32.Parse(companyId), createIssuedDocumentRequest);
                return (result.Data.DnUrl, result.Data.Id.Value);
            }
            catch (ApiException  e)
            {
                Logger.Error("Exception when calling IssuedDocumentsApi.CreateIssuedDocument: " + e.Message);
                throw e;
            }
        }

        public void DeleteDdtInCloudById(int id)
        {
            Logger.Info("Inizio invio a ApiFattureInCloud");
            Configuration config = new Configuration();
            config.BasePath = _configuration["ApiFattureInCloud:BasePath"];
            config.AccessToken = _ficAccessToken;
            var apiInstance = new IssuedDocumentsApi(config);
            var companyId = Int32.Parse(_configuration["ApiFattureInCloud:CompanyId"]);
            var documentId = id;
            try
            {
                // Delete Issued Document
                apiInstance.DeleteIssuedDocument(companyId, documentId);
            }
            catch (ApiException  e)
            {
                Logger.Error("Exception when calling IssuedDocumentsApi.DeleteIssuedDocument: " + e.Message);
                throw e;
            }
        }

        public string AddOrderCloud(OrderDto order)
        {
            Logger.Info("Inizio invio a ApiFattureInCloud");

            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointNewProduct"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);

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

        public string AddDdtInCloud(Ddt_In ddt, string SKU)
        {
            try
            {
                Logger.Info("Inizio invio a ApiFattureInCloud");
                
                var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointNewProduct"];
    
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);

                OrderAPI orderToSendInCloud = new OrderAPI()
                {
                    code = ddt.Code,
                    name = SKU,
                    description = ddt.Description,
                    prezzo_ivato = false,
                    net_price = ddt.Price_Uni.ToString("0.##"),
                    gross_price = "0",
                    net_cost = "0",
                    
                    measure = "",
                    category = "",
                    notes = "",
                    in_stock = true,
                    stock_initial = ddt.Number_Piece
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
            catch (Exception e)
            {
                Logger.Error(e.Message);
                throw new Exception("Errore durante l'invio a fatture in cloud.");
            }
        }
        
        public string EditDdtInCloud(Ddt_In ddt, string SKU)
        {
            try
            {
                Logger.Info("Inizio invio a ApiFattureInCloud");
                
                var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointUpdateProduct"];
    
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint + ddt.FC_Ddt_In_ID);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);

                OrderAPI orderToSendInCloud = new OrderAPI()
                {
                    code = ddt.Code,
                    name = SKU,
                    description = ddt.Description,
                    prezzo_ivato = false,
                    net_price = ddt.Price_Uni.ToString("0.##"),
                    gross_price = "0",
                    net_cost = "0",
                    
                    measure = "",
                    category = "",
                    notes = "",
                    in_stock = true,
                    stock_initial = ddt.Number_Piece
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
            catch (Exception e)
            {
                Logger.Error(e.Message);
                throw new Exception("Errore durante l'invio a fatture in cloud.");
            }
        }

        public string DeleteOrder(string productId)
        {
            Logger.Info("Inizio eliminazione da ApiFattureInCloud");

            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointDeleteProduct"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint + productId);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";
            httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);

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

            var apiEndpoint = _configuration["ApiFattureInCloud:ApiEndpointUpdateProduct"];

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiEndpoint + order.ID_FattureInCloud);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            httpWebRequest.Headers.Add("authorization", "Bearer " + _ficAccessToken);
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

    class default_vat
    {
        public int id { get; set; }
        public bool is_disabled { get; set; }
    }

    class ClientApi
    {
        public string type { get; set; }
        public string name { get; set; }
        public string vatNumber { get; set; }
        public string taxCode { get; set; }
        public string addressStreet { get; set; }
        public string addressPostalCode { get; set; }
        public string addressCity { get; set; }
        public string addressProvince { get; set; }
        
    }
}
