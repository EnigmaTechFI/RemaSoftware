using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IClientService
    {
        Client AddClient(Client customer);
        List<Client> GetAllClients();
        Client GetClient(int id);
        int GetTotalCustomerCount();
        bool DeleteById(int clientId);
        bool UpdateClient(Client clientModel);
        int GetClientIdFattureInCloudByRemaClientId(int remaClientId);
    }
}