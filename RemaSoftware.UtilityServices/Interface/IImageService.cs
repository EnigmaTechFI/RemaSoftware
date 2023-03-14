using System.Threading.Tasks;

namespace RemaSoftware.UtilityServices.Interface
{
    public interface IImageService
    {
        public Task<string> SavingOrderImage(string photo);
    }
}
