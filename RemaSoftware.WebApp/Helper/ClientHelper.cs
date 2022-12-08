using System;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.ClientViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using RemaSoftware.Domain.Data;
using RemaSoftware.UtilityServices.Dtos;
using RemaSoftware.UtilityServices.Exceptions;
using RemaSoftware.UtilityServices.FattureInCloud;
using RemaSoftware.WebApp.Converters;

namespace RemaSoftware.WebApp.Helper;

public class ClientHelper
{
    private readonly IClientService _clientService;
    private readonly IAPIFatturaInCloudService _ficService;
    private readonly ApplicationDbContext _dbContext;

    public ClientHelper(IClientService clientService, IAPIFatturaInCloudService ficService, ApplicationDbContext dbContext)
    {
        _clientService = clientService;
        _ficService = ficService;
        _dbContext = dbContext;
    }

    public async Task AddClient(ClientViewModel model)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
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
        
            var ficClientId = await _ficService.AddClient(ClientConverter.FromModelToFicApiDto(newClient));
            newClient.FC_ClientID = ficClientId;
            var addedClient = _clientService.AddClient(newClient);
            transaction.Commit();
        }
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
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            var ficClientId = _clientService.GetClientIdFattureInCloudByRemaClientId(clientId);
            _ficService.DeleteClient(ficClientId);
            return _clientService.DeleteById(clientId);
            
        }
    }

    public async Task<bool> EditClient(ClientViewModel model)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            var clientModel = Converters.ClientConverter.FromVmToModel(model);

            var ficClientId = _clientService.GetClientIdFattureInCloudByRemaClientId(model.ClientId);
            await _ficService.UpdateClient(ClientConverter.FromModelToFicApiDto(clientModel), ficClientId);
            
            var result = _clientService.UpdateClient(clientModel);
            transaction.Commit();
            return result;
        }
    }
}