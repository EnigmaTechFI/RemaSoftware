using System.Threading.Tasks;

namespace RemaSoftware.UtilityServices.FattureInCloud;

public interface IFicBaseHttp
{
    Task<bool> Delete(string url);
    Task<TModel> Get<TModel>(string url) where TModel : class;
    Task<TModel> Post<TModel>(string url, object data) where TModel : class;
    Task<TModel> Put<TModel>(string url, object data) where TModel : class; 
}