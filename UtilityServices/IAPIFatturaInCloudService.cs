using UtilityServices.Dtos;


namespace UtilityServices
{
    public interface IAPIFatturaInCloudService
    {
        public bool AddOrderCloud(OrderDto order);
    }
}
