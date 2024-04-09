using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.PriceViewModel;
using RemaSoftware.WebApp.Validation;
using System.Linq;

namespace RemaSoftware.WebApp.Helper
{
    public class PriceHelper
    {
        private readonly IPriceService _priceService;
        private readonly IConfiguration _configuration;
        private readonly PriceValidation _priceValidation;


        public PriceHelper(IPriceService priceService,  PriceValidation priceValidation, IConfiguration configuration)
        {
            _priceService = priceService;
            _priceValidation = priceValidation;
            _configuration = configuration;
        }

        public PriceListViewModel GetPriceListViewModel()
        {
            return new PriceListViewModel
            {
                Prices = _priceService.GetAllPrices()
            };
        }
        
        public List<Price> GetPrices(int productId)
        {
            try
            {
                if (productId == 0)
                {
                    return _priceService.GetAllPrices();
                }
                else
                {
                    return _priceService.GetAllPricesByProductId(productId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        
        public Task<Price> NewPrice(NewPriceViewModel model)
        {
            var prices = _priceService.GetAllPrices();
            
            var modelOperationIDs = model.Price.PriceOperation.Select(po => po.OperationID);

            var existingPrice = prices.FirstOrDefault(p =>
                p.PriceOperation.Select(po => po.OperationID).OrderBy(id => id).SequenceEqual(modelOperationIDs.OrderBy(id => id)) &&
                p.ProductID == model.Price.ProductID);
            
            if (existingPrice != null)
            {
                throw new Exception("Prezzo già registrato.");
            }
            
            model.Price.PriceVal = Decimal.Parse(model.PriceVal, new CultureInfo("it-IT")); 
            model.Price.CreationDate = DateTime.Now;
            
            var validation = _priceValidation.ValidatePrice(model.Price);
            if(validation != "")
                throw new Exception(validation);
            
            return Task.FromResult(_priceService.NewPrice(model.Price));
        }
        
        public Price GetPriceById(int Id)
        {
            return _priceService.GetPriceById(Id);
        }
        
        public string DeletePrice(int Id)
        {
            try
            {
                var price = _priceService.GetPriceAndPriceOperation(Id);
                return _priceService.DeletePrice(price);
            }
            catch(Exception e)
            {
                throw new Exception("Errore nell'eliminazione del prodotto");
            }
        }
        
        public async Task<string> UpdatePrice(NewPriceViewModel model)
        {
            try
            {
                if(model.Price.PriceVal == null || model.Price.Description == "")
                    throw new Exception("Impossibile effettuare la modifica");
                
                model.Price.PriceVal = Decimal.Parse(model.PriceVal, new CultureInfo("it-IT")); 
                model.Price.CreationDate = DateTime.Now;
                
                _priceService.UpdatePrice(model.Price);
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
