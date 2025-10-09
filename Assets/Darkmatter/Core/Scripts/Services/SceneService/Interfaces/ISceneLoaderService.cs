using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.InitiatorService;
using UnityEngine;

namespace Darkmatter.Core.Services.SceneService
{
    public interface ISceneLoaderService
    {
        void Initialize();
        UniTask<bool> TryLoadScene<TEnterData>(SceneType sceneType, TEnterData enterData, CancellationTokenSource cancellationTokenSource) where TEnterData : class, IInitiatorEnterData;        
        UniTask<bool> TryUnloadScene(SceneType sceneType, CancellationTokenSource cancellationTokenSource);
    }
}
