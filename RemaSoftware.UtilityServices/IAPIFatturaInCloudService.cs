using RemaSoftware.UtilityServices.Dtos;


namespace RemaSoftware.UtilityServices
{
    public interface IAPIFatturaInCloudService
    {
        public string AddOrderCloud(OrderDto order);
        public string DeleteOrder(string productId);
    }
}
