using System.Collections.Generic;
using RemaSoftware.ContextModels;

namespace RemaSoftware.DALServices
{
    public interface IClientService
    {
        List<Client> GetAllClients();
        Client GetClient(int id);
    }
}