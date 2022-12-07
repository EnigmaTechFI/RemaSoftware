using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.ClientViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using RemaSoftware.UtilityServices.Dtos;
using RemaSoftware.UtilityServices.FattureInCloud;

namespace RemaSoftware.WebApp.Helper;

public class ClientHelper
{
    private readonly IClientService _clientService;
    private readonly IAPIFatturaInCloudService _ficService;

    public ClientHelper(IClientService clientService, IAPIFatturaInCloudService ficService)
    {
        _clientService = clientService;
        _ficService = ficService;
    }

    public async Task AddClient(ClientViewModel model)
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
            P_Iva = model.P_Iva,
            Email = model.Email,
            Fax = model.Fax ?? "",
            PhoneNumber = model.PhoneNumber,
            Nation_ISO = ""
        };
        
        var addedClient = _clientService.AddClient(newClient);
        var ficClientId = await _ficService.AddClient(Converters.ClientConverter.FromModelToFicApiDto(newClient));
        addedClient.FC_ClientID = ficClientId;
        // TODO transazionalità (come per order ma da rivedere il rollback)
    }

    public List<Client> GetAllClients()
    {
        return _clientService.GetAllClients();
    }

    public ClientViewModel GetClient(int clientId)
    {
        var client = _clientService.GetClient(clientId);
        return Converters.ClientConverter.FroModelToVm(client);
    }

    public bool DeleteClient(int clientId)
    {
        return _clientService.DeleteById(clientId);
    }

    public bool EditClient(ClientViewModel model)
    {
        var clientModel = Converters.ClientConverter.FromVmToModel(model);
        return _clientService.UpdateClient(clientModel);
    }
}