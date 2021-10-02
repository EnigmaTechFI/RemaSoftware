using UtilityServices.Dtos;

namespace UtilityServices
{
    public interface IPdfService
    {
        byte[] GenerateOrderPdf(OrderDto order);
    }
}