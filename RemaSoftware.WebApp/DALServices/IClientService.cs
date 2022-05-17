using System.Collections.Generic;
using RemaSoftware.WebApp.ContextModels;

namespace RemaSoftware.WebApp.DALServices
{
    public interface IClientService
    {
        void AddClient(Client customer);
        List<Client> GetAllClients();
        Client GetClient(int id);
        int GetTotalCustomerCount();
    }
}