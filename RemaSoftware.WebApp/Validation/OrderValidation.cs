using RemaSoftware.WebApp.Models.OrderViewModel;
using System;
using System.Globalization;

namespace RemaSoftware.WebApp.Validation
{
    public class OrderValidation
    {
        public string ValidateNewOrderViewModelAndSetDefaultData(NewOrderViewModel model)
        {
            model.Ddt_In.DataOut = DateTime.Parse(model.Date, new CultureInfo("it-IT"));
            model.Ddt_In.Status = "A";
            model.Ddt_In.DataIn = DateTime.Now;
            model.Ddt_In.Number_Piece_Now = model.Ddt_In.Number_Piece - model.Ddt_In.NumberMissingPiece;
            try
            {
                model.Ddt_In.Price_Uni = model.Ddt_In.IsReso ? 0 : Decimal.Parse(model.Price, new CultureInfo("it-IT")); 
            }
            catch (Exception e)
            {
                return "Prezzo inserito non valido.";
            }

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
        
        public string ValidateDuplicateOrderViewModelAndSetDefaultData(DuplicateOrderViewModel model)
        {
            model.Ddt_In.DataOut = DateTime.Parse(model.Date, new CultureInfo("it-IT"));
            model.Ddt_In.Status = "A";
            model.Ddt_In.DataIn = DateTime.Now;
            model.Ddt_In.Number_Piece_Now = model.Ddt_In.Number_Piece - model.Ddt_In.NumberMissingPiece;
            try
            {
                model.Ddt_In.Price_Uni = model.Ddt_In.IsReso ? 0 : Decimal.Parse(model.Price, new CultureInfo("it-IT")); 
            }
            catch (Exception e)
            {
                return "Prezzo inserito non valido.";
            }

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
            model.Ddt_In.Number_Piece_Now = model.Ddt_In.Number_Piece - model.Ddt_In.NumberMissingPiece;
            try
            {
                model.Ddt_In.Price_Uni = model.Ddt_In.IsReso ? 0 : Decimal.Parse(model.Price, new CultureInfo("it-IT")); 
            }
            catch (Exception e)
            {
                return "Prezzo inserito non valido.";
            }
            

            if (string.IsNullOrEmpty(model.Ddt_In.Code))
                return "Inserire Codice DDT.";
            if (model.Ddt_In.Number_Piece <= 0)
                return "Inserire il numero di pezzi";
            if (string.IsNullOrEmpty(model.Ddt_In.Description))
                return "Inserire descrizione DDT.";
            if ((model.Ddt_In.Price_Uni < default(decimal) || model.Ddt_In.Price_Uni == null) && !model.Ddt_In.IsReso)
                return "Prezzo unitario mancante.";
            return "";
        }

        public string ValidateDDTSupplier(ExitToSupplierViewModel model)
        {
            try
            {
                model.DdtSupplier.Cost_Uni = Decimal.Parse(model.CostUni, new CultureInfo("it-IT")); 
            }
            catch (Exception e)
            {
                return "Prezzo inserito non valido.";
            }
            if (model.DdtSupplier.Number_Piece <= 0)
                return "Nessun pezzo inserito";
            if (model.DdtSupplier.SupplierID <= 0)
                return "Inserire fornitore.";
            if (model.BatchOperationID <= 0)
                return "Selezionare operazione";
            return "";
        }
    }
}
