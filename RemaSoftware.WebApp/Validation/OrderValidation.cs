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
            model.Ddt_In.Price_Uni = model.Ddt_In.IsReso ? 0 : model.Ddt_In.Price_Uni; 

            if (string.IsNullOrEmpty(model.Ddt_In.Code))
                return "Inserire Codice DDT.";
            if (model.Ddt_In.Number_Piece <= 0)
                return "Inserire il numero di pezzi";
            if (string.IsNullOrEmpty(model.Ddt_In.Description))
                return "Inserire descrizione DDT.";
            if ((model.Ddt_In.Price_Uni < default(decimal) || (model.Ddt_In.Price_Uni == null)) && !model.Ddt_In.IsReso)
                return "Prezzo unitario mancante.";
            if (model.Ddt_In.DataOut.Date < DateTime.Now.Date)
                return "Data di scadenza non valida.";
            if (model.OperationsSelected.Count == 0)
                return "Nessuna operazione inserita.";
            return "";
        }
        
        public string ValidateEditOrderViewModelAndSetDefaultData(NewOrderViewModel model)
        {
            model.Ddt_In.Number_Piece_Now = model.Ddt_In.Number_Piece;
            model.Ddt_In.Price_Uni = model.Ddt_In.IsReso ? 0 : model.Ddt_In.Price_Uni; 

            if (string.IsNullOrEmpty(model.Ddt_In.Code))
                return "Inserire Codice DDT.";
            if (model.Ddt_In.Number_Piece <= 0)
                return "Inserire il numero di pezzi";
            if (string.IsNullOrEmpty(model.Ddt_In.Description))
                return "Inserire descrizione DDT.";
            if ((model.Ddt_In.Price_Uni < default(decimal) || (model.Ddt_In.Price_Uni == null)) && !model.Ddt_In.IsReso)
                return "Prezzo unitario mancante.";
            if (model.Ddt_In.DataOut.Date < DateTime.Now.Date)
                return "Data di scadenza non valida.";
            return "";
        }
    }
}
