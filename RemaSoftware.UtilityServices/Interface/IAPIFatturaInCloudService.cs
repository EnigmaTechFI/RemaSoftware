using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Dtos;
using UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices.Interface
{
    public interface IAPIFatturaInCloudService
    {
        public string AddOrderCloud(OrderDto order);
        public string AddDdtInCloud(Ddt_In ddt, string SKU, decimal PriceUni);
        public string DeleteOrder(string productId);
        public int AddClientCloud(Client client);
        public void UpdateClientCloud(Client client);
        public (string, int) CreateDdtInCloud(Ddt_Out ddtOut);
        public void DeleteDdtInCloudById(int id);
    }
}
