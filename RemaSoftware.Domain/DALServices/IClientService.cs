using System.Collections.Generic;
using RemaSoftware.Domain.ContextModels;

namespace RemaSoftware.Domain.DALServices
{
    public interface IClientService
    {
        void AddClient(Client customer);
        List<Client> GetAllClients();
        Client GetClient(int id);
        int GetTotalCustomerCount();
    }
}