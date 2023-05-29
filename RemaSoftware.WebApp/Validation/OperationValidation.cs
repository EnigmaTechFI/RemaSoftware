using System;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Validation;

public class OperationValidation
{
    public string ValidateNewOperation(Operation operation)
    {
        if (string.IsNullOrEmpty(operation.Name))
            throw new Exception("Inserire nome operazione.");
        if (string.IsNullOrEmpty(operation.Description))
            throw new Exception("Inserire descrizione operazione.");
        if (operation.Name == OtherConstants.COQ || operation.Name == OtherConstants.EXTRA)
            throw new Exception("Nome non utilizzabile.");
        return "";
    }
}