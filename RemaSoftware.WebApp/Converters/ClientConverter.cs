using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Dtos;
using RemaSoftware.WebApp.Models.ClientViewModel;

namespace RemaSoftware.WebApp.Converters;

public static class ClientConverter
{
    public static Client FromVmToModel(ClientViewModel vm)
    {
        return new Client
        {
            ClientID = vm.ClientId,
            Name = vm.Name,
            P_Iva = vm.P_Iva,
            Street = vm.Street,
            StreetNumber = vm.StreetNumber,
            Cap = vm.Cap,
            City = vm.City,
            Province= vm.Province, 
            Nation = vm.Nation,
            Email = vm.Email,
            PhoneNumber = vm.PhoneNumber,
            Fax = vm.Fax,
        };
    }

    public static ClientViewModel FroModelToVm(Client model)
    {
        return new ClientViewModel()
        {
            ClientId = model.ClientID,
            Name = model.Name,
            P_Iva = model.P_Iva,
            Street = model.Street,
            StreetNumber = model.StreetNumber,
            Cap = model.Cap,
            City = model.City,
            Province = model.Province,
            Nation = model.Nation,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Fax = model.Fax
        };
    }

    public static ClientDto FromModelToFicApiDto(Client model)
    {
        return new ClientDto()
        {
            Name = model.Name,
            P_Iva = model.P_Iva,
            Street = model.Street + ", " + model.StreetNumber,
            Cap = model.Cap,
            City = model.City,
            Province = model.Province,
            Nation = model.Nation,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Fax = model.Fax
        };
    }
}