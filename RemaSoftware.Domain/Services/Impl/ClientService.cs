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

        public ClientService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddClient(Client customer)
        {
            _dbContext.Add(customer);
            _dbContext.SaveChanges();
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

        public void UpdateClient(Client client)
        {
            try
            {
                _dbContext.Clients.Update(client);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Errore SQL durante l'aggiornamento.");
            }
        }
        
    }
}