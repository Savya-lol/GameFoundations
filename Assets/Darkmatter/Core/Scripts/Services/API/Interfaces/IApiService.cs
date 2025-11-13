
using Cysharp.Threading.Tasks;

namespace Darkmatter.Core.Services.API
{
    public interface IApiService
    {
        UniTask<T> GetAsync<T>(string url);
        UniTask<T> PostAsync<T>(string url, object body);
    }
}
