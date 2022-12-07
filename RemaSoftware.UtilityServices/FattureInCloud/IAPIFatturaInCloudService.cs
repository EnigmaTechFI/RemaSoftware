using System.Threading.Tasks;
using RemaSoftware.UtilityServices.Dtos;
using UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices.FattureInCloud
{
    public interface IAPIFatturaInCloudService
    {
        // Order
        public string AddOrderCloud(OrderDto order);
        public string DeleteOrder(string productId);
        public bool UpdateOrderCloud(OrderToUpdateDto order);
        
        // Client
        public Task<int> AddClient(ClientDto client);
    }
}
