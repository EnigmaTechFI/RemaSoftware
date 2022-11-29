using RemaSoftware.UtilityServices.Dtos;
using UtilityServices.Dtos;

namespace RemaSoftware.UtilityServices
{
    public interface IAPIFatturaInCloudService
    {
        public string AddOrderCloud(OrderDto order);
        public string DeleteOrder(string productId);
        public bool UpdateOrderCloud(OrderToUpdateDto order);
    }
}
