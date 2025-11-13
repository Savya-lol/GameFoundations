using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.InitiatorService;
using Darkmatter.Core.Services.InitiatorService.Scenes;
using Darkmatter.Core.Services.LoggingService;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Darkmatter.Core.Services.SceneService
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly ISceneInitiatorsService _sceneInitiatorsService;
        private readonly HashSet<string> _loadedScenes = new();
        private readonly HashSet<string> _loadingScenes = new();

        [Inject]
        public SceneLoaderService(ISceneInitiatorsService sceneInitiatorsService)
        {
            _sceneInitiatorsService = sceneInitiatorsService;
        }

        public void Initialize()
        {
            AddOpenedScenesToLoadedHashset();
        }

        public async UniTask<bool> TryLoadScene(string sceneName, CancellationTokenSource cancellationTokenSource)
        {
            var isSceneAlreadyLoaded = _loadedScenes.Contains(sceneName);

            if (isSceneAlreadyLoaded)
            {
                LogService.LogError($"scene:{sceneName} is already Loaded");
                return false;
            }

            var isSceneAlreadyLoading = _loadingScenes.Contains(sceneName);

            if (isSceneAlreadyLoading)
            {
                LogService.LogError($"scene:{sceneName} is already Loading");
                return false;
            }

            await LoadScene(sceneName, cancellationTokenSource);
            return true;
        }

        public async UniTask<bool> TryLoadScene<TEnterData>(SceneType sceneType, TEnterData enterData, CancellationTokenSource cancellationTokenSource) where TEnterData : class, IInitiatorEnterData
        {
            if (!await TryLoadScene(sceneType.ToString(), cancellationTokenSource))
            {
                return false;
            }

            await _sceneInitiatorsService.InvokeInitiatorLoadEntryPoint(sceneType, enterData, cancellationTokenSource);
            await _sceneInitiatorsService.InvokeInitiatorStartEntryPoint(sceneType, enterData, cancellationTokenSource);
            return true;
        }

        public async UniTask<bool> TryUnloadScene(SceneType sceneType, CancellationTokenSource cancellationTokenSource)
        {
            var sceneName = sceneType.ToString();
            var isSceneAlreadyLoaded = _loadedScenes.Contains(sceneName);

            if (!isSceneAlreadyLoaded)
            {
                LogService.LogError($"scene:{sceneName} cant be unloaded as it is not Loaded");
                return false;
            }

            var isSceneAlreadyLoading = _loadingScenes.Contains(sceneName);

            if (isSceneAlreadyLoading)
            {
                LogService.LogError($"scene:{sceneName} cant be unloaded as it during Loading");
                return false;
            }

            await UnloadScene(sceneType, cancellationTokenSource);
            return true;
        }

        private void AddOpenedScenesToLoadedHashset()
        {
            var countLoaded = SceneManager.sceneCount;

            for (var i = 0; i < countLoaded; i++)
            {
                var sceneName = SceneManager.GetSceneAt(i).name;

                if (!_loadedScenes.Contains(sceneName))
                {
                    _loadedScenes.Add(sceneName);
                }
            }
        }

        private async Awaitable LoadScene(string sceneName, CancellationTokenSource cancellationTokenSource)
        {
            _loadingScenes.Add(sceneName);
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            _loadingScenes.Remove(sceneName);
            _loadedScenes.Add(sceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private async UniTask UnloadScene(SceneType sceneType, CancellationTokenSource cancellationTokenSource)
        {
            await _sceneInitiatorsService.InvokeInitiatorExitPoint(sceneType, cancellationTokenSource);
            var sceneName = sceneType.ToString();
            await SceneManager.UnloadSceneAsync(sceneName);
            _loadedScenes.Remove(sceneName);
        }
    }
}