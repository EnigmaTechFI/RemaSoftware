using UtilityServices.Dtos;


namespace UtilityServices
{
    public interface IAPIFatturaInCloudService
    {
        public string AddOrderCloud(OrderDto order);
        public string DeleteOrder(string productId);
        public string UpdateOrderCloud(OrderToUpdateDto order);
    }
}
