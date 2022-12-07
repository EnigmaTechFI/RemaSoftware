using RemaSoftware.Domain.Models;
using System;

namespace RemaSoftware.WebApp.Validation
{
    public class ProductValidation
    {
        public string ValidateProduct(Product product)
        {

            if (string.IsNullOrEmpty(product.SKU))
                throw new Exception("Inserire codice prodotto.");
            if (string.IsNullOrEmpty(product.Image_URL))
                throw new Exception("Acquisire foto prodotto.");
            if (product.ClientID < 1 || (product.ClientID == null))
                throw new Exception("Selezionare cliente.");
            if (string.IsNullOrEmpty(product.Name))
                throw new Exception("Inserire nome prodotto.");
            return "";
        }
    }
}
