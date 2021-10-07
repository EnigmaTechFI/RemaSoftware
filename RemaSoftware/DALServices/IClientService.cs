using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IClientService
    {
        void AddClient(Client customer);
        List<Client> GetAllClients();
        Client GetClient(int id);
        int GetTotalCustomerCount();
    }
}