using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IPriceService
    {
        public List<Price> GetAllPrices();

        public Price NewPrice(Price price);
        
        public Price GetPriceById(int Id);
        
        public Price GetPriceAndPriceOperation(int Id);

        public string DeletePrice(Price price);
        
        public Price UpdatePrice(Price price);
        
        public List<Price> GetAllPricesByProductId(int productId);
        
    }
}
