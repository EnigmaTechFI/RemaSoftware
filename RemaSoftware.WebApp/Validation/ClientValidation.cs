using System.ComponentModel.DataAnnotations;
using RemaSoftware.WebApp.Models.ClientViewModel;

namespace RemaSoftware.WebApp.Validation;

public class ClientValidation
{
    public (bool Result, string Errors) ValideateClientUser(AddOrdUpdateClientUserViewModel model)
    {
        var emailValidator = new EmailAddressAttribute();
        if (string.IsNullOrEmpty(model.Email) || !emailValidator.IsValid(model.Email))
            return (false, "Email non valida.");
        if(string.IsNullOrEmpty(model.Name))
            return (false, "Il campo nome non può essere vuoto.");
        if(string.IsNullOrEmpty(model.Surname))
            return (false, "Il campo cognome non può essere vuoto.");

        return (true, string.Empty);
    }
}