using System.Collections.Generic;
using System.Linq;
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
    }
}