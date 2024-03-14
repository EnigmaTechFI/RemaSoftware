using RemaSoftware.Domain.Models;
using System;

namespace RemaSoftware.WebApp.Validation
{
    public class PriceValidation
    {
        public string ValidatePrice(Price price)
        {

            if (string.IsNullOrEmpty(price.ProductID.ToString()))
                throw new Exception("Inserire prodotto.");
            if (string.IsNullOrEmpty(price.OperationID.ToString()))
                throw new Exception("Inserire operazione.");
            if (string.IsNullOrEmpty(price.Description))
                throw new Exception("Inserire descrizione.");
            if (string.IsNullOrEmpty(price.PriceVal.ToString()))
                throw new Exception("Inserire valore prezzo.");
            return "";
        }
    }
}
