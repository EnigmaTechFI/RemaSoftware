using Microsoft.EntityFrameworkCore;
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
            var client = _dbContext.Clients.Include(s => s.Ddt_Template).SingleOrDefault(i => i.ClientID == id);
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

        public int GetClientIdByUserId(string id)
        {
            return _dbContext.UserClients.SingleOrDefault(s => s.MyUserID == id).ClientID;
        }

        public List<Ddt_Template> GetDdts_Templates()
        {
            return _dbContext.Ddt_Templates.ToList();
        }
        
        public List<Ddt_In> ClientAccountingInfo(int Id)
        {
            return _dbContext.Ddts_In
                .Include(d => d.Product)
                .ThenInclude(s => s.Client)
                .Include(b => b.SubBatch)
                .ThenInclude(s => s.Batch)
                .Where(s => s.Product.ClientID == Id)
                .ToList();
        }
    }
}