using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IClientService
    {
        void AddClient(Client customer);
        List<Client> GetAllClients();
        Client GetClient(int id);
        int GetTotalCustomerCount();
        void UpdateClient(Client client);
        int GetClientIdByUserId(string id);
        List<Ddt_Template> GetDdts_Templates();
        List<Ddt_In> ClientAccountingInfo(int Id);

    }
}