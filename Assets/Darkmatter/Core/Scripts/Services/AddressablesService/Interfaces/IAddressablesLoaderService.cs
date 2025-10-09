using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkmatter.Core.Services.AddressablesService
{
    public interface IAddressablesLoaderService
    {
        UniTask<T> LoadAsync<T>(string address, CancellationTokenSource cancellationTokenSource) where T : Object;
        void Release(string address);
        void ReleaseAll();
        bool IsLoaded(string address);
    }
}