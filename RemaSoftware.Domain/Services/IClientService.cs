using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IClientService
    {
        void AddClient(Client customer);
        List<Client> GetAllClients();
        Client GetClient(int id);
        int GetTotalCustomerCount();
    }
}