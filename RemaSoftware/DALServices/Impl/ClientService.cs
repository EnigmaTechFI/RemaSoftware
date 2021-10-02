using System.Collections.Generic;
using System.Linq;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;

namespace RemaSoftware.DALServices.Impl
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _dbContext;

        public ClientService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public List<Client> GetAllClients()
        {
            return _dbContext.Clients.ToList();
        }
    }
}