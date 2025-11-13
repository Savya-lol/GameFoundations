using UnityEngine;
using System.Net.Http;
using Cysharp.Threading.Tasks;

namespace Darkmatter.Core.Services.API
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();
        }

        public async UniTask<T> GetAsync<T>(string url)
        {
            var json = await _client.GetStringAsync(url);
            return JsonUtility.FromJson<T>(json);
        }

        public async UniTask<T> PostAsync<T>(string url, object body)
        {
            var payload = JsonUtility.ToJson(body);
            var result = await _client.PostAsync(url, new StringContent(payload));
            var json = await result.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<T>(json);
        }
    }
}
