using System.Collections.Generic;
using System.Linq;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ClientService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Client AddClient(Client customer)
        {
            var addedClient = _dbContext.Add(customer);
            _dbContext.SaveChanges();
            return addedClient.Entity;
        }

        public List<Client> GetAllClients()
        {
            return _dbContext.Clients.ToList();
        }

        public Client GetClient(int id)
        {
            var client = _dbContext.Clients.SingleOrDefault(i => i.ClientID == id);
            return client;
        }

        public int GetTotalCustomerCount()
        {
            return _dbContext.Clients.Count();
        }

        public bool DeleteById(int clientId)
        {
            try
            {
                var hasProducts = _dbContext.Products.Any(a => a.ClientID == clientId);
                if (hasProducts)
                {
                    var client = _dbContext.Clients.SingleOrDefault(sd => sd.ClientID == clientId);
                    //client.IsDeleted = true;
                    _dbContext.Clients.Update(client);
                }
                else
                    _dbContext.Clients.Remove(new Client { ClientID = clientId});
                
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante l'eliminazione del cliente: {clientId}. ", e);
                return false;
            }
        }

        public bool UpdateClient(Client clientModel)
        {
            try
            {
                _dbContext.Clients.Update(clientModel);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Errore durante l'update del cliente: {clientModel.ClientID}. ", e);
                return false;
            }
        }

        public int GetClientIdFattureInCloudByRemaClientId(int remaClientId)
        {
            if (remaClientId <= 0) throw new ArgumentException("Parametro RemaClientId obbligaotrio.");
            return _dbContext.Clients.Single(sd => sd.ClientID == remaClientId).FC_ClientID;
        }
    }
}