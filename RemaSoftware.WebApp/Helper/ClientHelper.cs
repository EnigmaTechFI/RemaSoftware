using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.ClientViewModel;

namespace RemaSoftware.WebApp.Helper;

public class ClientHelper
{
    private readonly IClientService _clientService;
    public ClientHelper(IClientService clientService)
    {
        _clientService = clientService;
    }

    public void AddClient(ClientViewModel model)
    {
        var newClient = new Client
        {
            Name = model.Name,
            StreetNumber = model.StreetNumber,
            Street = model.Street,
            Cap = model.Cap,
            City = model.City,
            Nation = model.Nation,
            Province = model.Province,
            P_Iva = model.P_Iva
        };
        _clientService.AddClient(newClient);
    }
}