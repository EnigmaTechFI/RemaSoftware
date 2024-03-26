using RemaSoftware.Domain.Models;
using System;
using System.Linq;

namespace RemaSoftware.WebApp.Validation
{
    public class PriceValidation
    {
        public string ValidatePrice(Price price)
        {

            if (string.IsNullOrEmpty(price.ProductID.ToString()))
                throw new Exception("Inserire prodotto.");
            if (string.IsNullOrEmpty(price.PriceOperation[0]?.ToString()))
                throw new Exception("Inserire almeno una operazione.");
            if (string.IsNullOrEmpty(price.Description))
                throw new Exception("Inserire descrizione.");
            if (string.IsNullOrEmpty(price.PriceVal.ToString()))
                throw new Exception("Inserire valore prezzo.");
            return "";
        }
    }
}
