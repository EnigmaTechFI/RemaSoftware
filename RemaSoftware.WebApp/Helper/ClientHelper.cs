using System;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.ClientViewModel;
using System.Collections.Generic;
using System.Linq;
using RemaSoftware.Domain.Data;
using RemaSoftware.UtilityServices.Interface;


namespace RemaSoftware.WebApp.Helper;

public class ClientHelper
{
    private readonly IClientService _clientService;
    private readonly IAPIFatturaInCloudService _apiFatturaInCloudService;
    private readonly ApplicationDbContext _dbContext;
    public ClientHelper(IClientService clientService, IAPIFatturaInCloudService apiFatturaInCloudService, ApplicationDbContext dbContext)
    {
        _clientService = clientService;
        _apiFatturaInCloudService = apiFatturaInCloudService;
        _dbContext = dbContext;
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
                Nation_ISO = "",
                SDI = model.SDI,
                Pec = model.Pec
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
            Client = _clientService.GetClient(id),
        };
    }

    public List<Client> GetAllClients()
    {
        return _clientService.GetAllClients();
    }
    
    public InfoClientViewModel GetInfoClientModel(int id)
    {
        return new InfoClientViewModel()
        {
            Client = _clientService.GetClient(id),
            MyUsers = _dbContext.UserClients.Where(i => i.ClientID == id).Select(i => i.MyUser).ToList()
        };
    }
    
    public bool DeleteClientById(int ClientID)
    {
        try
        {
            List<MyUser> users;
            
            var client = _dbContext.Clients.SingleOrDefault(i => i.ClientID == ClientID);
            users = _dbContext.UserClients.Where(i => i.ClientID == ClientID).Select(i => i.MyUser).ToList();

            _dbContext.Remove(client);
            for(int i=0; i<users.Count; i++)
            {
                _dbContext.Remove(users[i]);
            }
            _dbContext.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

}
