using System.Collections.Generic;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices.Interface
{
    public interface IAPIFatturaInCloudService
    {
        public string AddDdtInCloud(Ddt_In ddt, string SKU);
        public string EditDdtInCloud(Ddt_In ddt, string SKU);
        public string DeleteOrder(string productId);
        public int AddClientCloud(Client client);
        public void UpdateClientCloud(Client client);
        public (string, int) CreateDdtInCloud(Ddt_Out ddtOut);
        public void DeleteDdtInCloudById(int id);
        public List<Supplier> GetListSuppliers();
    }
}
