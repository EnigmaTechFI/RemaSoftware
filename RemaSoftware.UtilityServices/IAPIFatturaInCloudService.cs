using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Dtos;
using UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices
{
    public interface IAPIFatturaInCloudService
    {
        public string AddOrderCloud(OrderDto order);
        public string DeleteOrder(string productId);
        public bool UpdateOrderCloud(OrderToUpdateDto order);
        public int AddClientCloud(Client client);
        public void UpdateClientCloud(Client client);
    }
}
