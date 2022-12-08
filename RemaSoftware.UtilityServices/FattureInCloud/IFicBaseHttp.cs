using System.Threading.Tasks;
using RemaSoftware.UtilityServices.FicResponses;

namespace RemaSoftware.UtilityServices.FattureInCloud;

public interface IFicBaseHttp
{
    Task<bool> Delete(string url);
    Task<TModel> Get<TModel>(string url) where TModel : class;
    Task<TModel> Post<TModel>(string url, object data) where TModel : BaseErrorFicResponse;
    Task<TModel> Put<TModel>(string url, object data) where TModel : BaseErrorFicResponse;
}