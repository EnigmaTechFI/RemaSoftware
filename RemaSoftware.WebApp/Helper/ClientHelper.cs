using System;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.ClientViewModel;
using System.Collections.Generic;
using NLog;
using RemaSoftware.UtilityServices;

namespace RemaSoftware.WebApp.Helper;

public class ClientHelper
{
    private readonly IClientService _clientService;
    private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
    public ClientHelper(IClientService clientService, IAPIFatturaInCloudService apiFatturaInCloudService)
    {
        _clientService = clientService;
        _apiFatturaInCloudService = apiFatturaInCloudService;
    }

    public void UpdateClient(UpdateClientViewModel model)
    {
        try
        {
            _clientService.UpdateClient(model.Client);
            _apiFatturaInCloudService.UpdateClientCloud(model.Client);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void AddClient(ClientViewModel model)
    {
        try
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


            newClient.FC_ClientID = _apiFatturaInCloudService.AddClientCloud(newClient);
            _clientService.AddClient(newClient);
        }
        catch (Exception e)
        {
            throw e;
        }
        
    }

    public UpdateClientViewModel GetUpdateClientModel(int id)
    {
        return new UpdateClientViewModel()
        {
            Client = _clientService.GetClient(id)
        };
    }

    public List<Client> GetAllClients()
    {
        return _clientService.GetAllClients();
    }
}