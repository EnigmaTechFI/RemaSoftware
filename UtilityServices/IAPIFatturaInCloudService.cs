using UtilityServices.Dtos;


namespace UtilityServices
{
    public interface IAPIFatturaInCloudService
    {
        public string AddOrderCloud(OrderDto order);
    }
}
