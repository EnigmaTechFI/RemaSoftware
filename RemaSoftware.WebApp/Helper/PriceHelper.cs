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
                Prices = GetAllPrices()
            };
        }
        
        public List<Price> GetAllPrices()
        {
            try
            {
                return _priceService.GetAllPrices();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        
        public Task<Price> NewPrice(NewPriceViewModel model)
        {
            var prices = _priceService.GetAllPrices();

            var modelOperationIDs = model.Price.Price_Operation.Select(po => po.OperationID);

            var existingPrice = prices.FirstOrDefault(p =>
                p.Price_Operation.Any(po => modelOperationIDs.Contains(po.OperationID)) &&
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
                var price = _priceService.GetPriceById(Id);
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
                var validation = _priceValidation.ValidatePrice(model.Price);
                if(validation != "")
                    throw new Exception(validation);
                
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
