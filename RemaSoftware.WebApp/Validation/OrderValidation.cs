using RemaSoftware.WebApp.Models.OrderViewModel;
using System;

namespace RemaSoftware.WebApp.Validation
{
    public class OrderValidation
    {
        public string ValidateNewOrderViewModelAndSetDefaultData(NewOrderViewModel model)
        {
            model.Ddt_In.Status = "A";
            model.Ddt_In.DataIn = DateTime.Now;
            model.Ddt_In.Number_Piece_Now = model.Ddt_In.Number_Piece;
            model.uni_price = model.Ddt_In.IsReso ? 0 : model.uni_price; 

            if (string.IsNullOrEmpty(model.Ddt_In.Code))
                return "Inserire Codice DDT.";
            if (model.Ddt_In.Number_Piece <= 0)
                return "Inserire il numero di pezzi";
            if (string.IsNullOrEmpty(model.Ddt_In.Description))
                return "Inserire descrizione DDT.";
            if ((model.uni_price < default(decimal) || (model.uni_price == null)) && !model.Ddt_In.IsReso)
                return "Prezzo unitario mancante.";
            if (model.Ddt_In.DataOut <= DateTime.Now)
                return "Data di scadenza non valida.";
            return "";
        }
    }
}
