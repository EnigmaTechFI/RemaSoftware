﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using It.FattureInCloud.Sdk.Api;
using It.FattureInCloud.Sdk.Client;
using It.FattureInCloud.Sdk.Model;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Interface;
using Supplier = RemaSoftware.Domain.Models.Supplier;

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
                    email = client.Email ?? "",
                    address_city = client.City ?? "",
                    address_street = client.Street + ", " + client.StreetNumber,
                    address_province = client.Province?? "",
                    address_postal_code = client.Cap?? "",
                    tax_code = client.P_Iva?? "",
                    vat_number = client.P_Iva?? "",
                    ei_code = client.SDI ?? "",
                    certified_email = client.Pec ?? ""
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
                Logger.Error(e, e.Message);
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
                    email = client.Email ?? "",
                    address_city = client.City ?? "",
                    address_street = client.Street + ", " + client.StreetNumber,
                    address_province = client.Province?? "",
                    address_postal_code = client.Cap?? "",
                    tax_code = client.P_Iva?? "",
                    vat_number = client.P_Iva?? "",
                    ei_code = client.SDI ?? "",
                    certified_email = client.Pec ?? ""
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
                Logger.Error(e, e.Message);
                throw new Exception("Errore durante l'aggiornamento del cliente a FattureInCloud.");
            }
        }

        public async Task<(string, int, string)> CreateDdtInCloud(Ddt_Out ddtOut)
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

                var description = "";
                switch (item.TypePieces)
                {
                    case PiecesType.BUONI:
                        description = item.Ddt_In.Description;
                        break;
                    case PiecesType.MANCANTI:
                        description = item.TypePieces + "\n" + item.Ddt_In.Description;
                        break;
                    case PiecesType.SCARTI:
                        description =  item.TypePieces+ "\n" + item.Ddt_In.Description;
                        break;
                    case PiecesType.PERSI:
                        description =  "Mancanti\n" + item.Ddt_In.Description;
                        break;
                    case PiecesType.ZAMA:
                        description =  "Zama scoperto\n" + item.Ddt_In.Description;
                        break;
                    case PiecesType.RESOSCARTO:
                        description =  "Reso scarto non lavorato\n" + item.Ddt_In.Description;
                        break;
                }

                var p = products.SingleOrDefault(s => s.Code == item.Ddt_In.Code && s.Description == description);
                if (p != null)
                {
                    p.Qty += item.NumberPieces;
                }
                else
                {
                    
                    products.Add(new IssuedDocumentItemsListItem()
                    {
                        ProductId = Int32.Parse(item.Ddt_In.FC_Ddt_In_ID),
                        Qty = item.NumberPieces,
                        Name = item.Ddt_In.Product.SKU,
                        Code = item.Ddt_In.Code,
                        Description = description,
                        Stock = true,
                        NetPrice = item.TypePieces == PiecesType.BUONI || item.TypePieces == PiecesType.SCARTI ? item.Ddt_In.Price_Uni : 0,
                        Vat = new VatType()
                        {
                            Id = 0,
                            Value = 22
                        }
                    
                    });
                }
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
                    DeliveryNoteTemplate = new DocumentTemplate()
                    {
                        Name = ddtOut.Client.Ddt_Template.Name,
                        Id = ddtOut.Client.Ddt_Template.FC_Template_ID
                    }
                }
            };
            try
            {
                CreateIssuedDocumentResponse result = await apiInstance.CreateIssuedDocumentAsync(Int32.Parse(companyId), createIssuedDocumentRequest);
                if (result.Data.Id.HasValue)
                    return (result.Data.DnUrl, result.Data.Id.Value, $"{result.Data.Number}/{DateTime.Now.Year}");
                return (result.Data.DnUrl, 0, $"{result.Data.Number}/{DateTime.Now.Year}");
            }
            catch (ApiException  e)
            {
                Logger.Error("Exception when calling IssuedDocumentsApi.CreateIssuedDocument: " + e.Message);
                throw new Exception("Errore durante la creazione della ddt. Si prega di riprovare.");
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

        public List<Supplier> GetListSuppliers()
        {
            Logger.Info("Inizio recupero fornitori FattureInCloud");
            Configuration config = new Configuration();
            config.BasePath = _configuration["ApiFattureInCloud:BasePath"];
            config.AccessToken = _ficAccessToken;
            var apiInstance = new SuppliersApi(config);
            var companyId = Int32.Parse(_configuration["ApiFattureInCloud:CompanyId"]);
            var suppliers = new List<Supplier>();
            try
            {
                var result = apiInstance.ListSuppliers(companyId, null, "detailed", null, 1, 100);
                foreach (var item in result.Data)
                {
                    suppliers.Add(new Supplier()
                    {
                        FC_SupplierID = item.Id.Value,
                        Cap = item.AddressPostalCode,
                        Street = item.AddressStreet,
                        City = item.AddressCity,
                        Pec = item.CertifiedEmail,
                        Email = item.Email,
                        PhoneNumber = item.Phone,
                        Province = item.AddressProvince,
                        Fax = item.Fax,
                        Name = item.Name,
                        P_Iva = item.TaxCode
                    });
                }
                
                for (int i = 2; i < result.LastPage; i++)
                {
                    result = apiInstance.ListSuppliers(companyId, null, "detailed", null, i, 100);
                    foreach (var item in result.Data)
                    {
                        suppliers.Add(new Supplier()
                        {
                            FC_SupplierID = item.Id.Value,
                            Cap = item.AddressPostalCode,
                            Street = item.AddressStreet,
                            City = item.AddressCity,
                            Pec = item.CertifiedEmail,
                            Email = item.Email,
                            PhoneNumber = item.Phone,
                            Province = item.AddressProvince,
                            Fax = item.Fax,
                            Name = item.Name,
                            P_Iva = item.TaxCode
                        });
                    }
                }
                
                
            }
            catch (ApiException  e)
            {
                Logger.Error("Exception when calling IssuedDocumentsApi.DeleteIssuedDocument: " + e.Message);
                throw e;
            }

            return suppliers;
        }

        public (string, string, int) CreateDdtSupplierCloud(Ddt_Supplier ddtSupplier, Supplier supplier)
        {
            Logger.Info("Inizio invio a ApiFattureInCloud");
            Configuration config = new Configuration();
            config.BasePath = _configuration["ApiFattureInCloud:BasePath"];
            config.AccessToken = _ficAccessToken;
            var apiInstance = new IssuedDocumentsApi(config);
            var companyId = _configuration["ApiFattureInCloud:CompanyId"];
            var products = new List<IssuedDocumentItemsListItem>();
            foreach (var item in ddtSupplier.DdtSupplierAssociations)
            {
                products.Add(new IssuedDocumentItemsListItem()
                {
                    ProductId = Int32.Parse(item.Ddt_In.FC_Ddt_In_ID),
                    Qty = item.NumberPieces,
                    Name = item.Ddt_In.Product.Name,
                    Code = item.Ddt_In.Code,
                    Description = item.Ddt_In.Product.SKU + "\n" + item.Ddt_In.Description ,
                    Stock = false,
                    NetPrice = 0,
                    Vat = new VatType()
                    {
                        Id = 0,
                        Value = 22
                    }
                });
            }
            var createIssuedDocumentRequest = new CreateIssuedDocumentRequest()
            {
                Data = new IssuedDocument()
                {
                    Type = IssuedDocumentType.DeliveryNote,
                    Entity = new Entity()
                    {
                        Name = supplier.Name,
                        AddressCity = supplier.City ?? "",
                        AddressProvince = supplier.Province ?? "",
                        AddressStreet = supplier.Street ?? "",
                        AddressPostalCode = supplier.Cap ?? "",
                        VatNumber = supplier.P_Iva,
                        TaxCode = supplier.P_Iva
                    },
                    ItemsList = products,
                }
            };
            try
            {
                CreateIssuedDocumentResponse result = apiInstance.CreateIssuedDocument(Int32.Parse(companyId), createIssuedDocumentRequest);
                return (result.Data.DnUrl, result.Data.DnNumber + "/" + DateTime.Now.Year, result.Data.Id.Value);
            }
            catch (ApiException  e)
            {
                Logger.Error("Exception when calling IssuedDocumentsApi.CreateIssuedDocument: " + e.Message);
                throw e;
            }
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
                    net_price = ddt.Price_Uni,
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
                Logger.Error(e, e.Message);
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
                    net_price = ddt.Price_Uni,
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
                Logger.Error(e, e.Message);
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
        public decimal net_price { get; set; }
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
        public string email { get; set; }
        public string vat_number { get; set; }
        public string tax_code { get; set; }
        public string address_street { get; set; }
        public string address_postal_code { get; set; }
        public string address_city { get; set; }
        public string address_province { get; set; }
        public string ei_code { get; set; }
        public string certified_email { get; set; }
        
    }
}
