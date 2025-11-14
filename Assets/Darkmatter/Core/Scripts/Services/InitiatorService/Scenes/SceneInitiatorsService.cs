using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.SceneService;
using Darkmatter.Core.Services.LoggingService;

namespace Darkmatter.Core.Services.InitiatorService.Scenes
{
    public class SceneInitiatorsService : ISceneInitiatorsService
    {
        private readonly Dictionary<SceneType, ISceneInitiator> _initiators = new();

        public void RegisterInitiator(ISceneInitiator sceneInitiator)
        {
            if (_initiators.ContainsKey(sceneInitiator.SceneType))
            {
                LogService.LogWarning($"Initiator for {sceneInitiator.SceneType} is already registered");
                return;
            }
            
            _initiators[sceneInitiator.SceneType] = sceneInitiator;
        }

        public void UnregisterInitiator(ISceneInitiator sceneInitiator)
        {
            if (_initiators.ContainsKey(sceneInitiator.SceneType))
            {
                _initiators.Remove(sceneInitiator.SceneType);
            }
        }

        public async UniTask InvokeInitiatorLoadPoint(SceneType sceneType, IInitiatorEnterData enterDataObject, CancellationTokenSource cancellationTokenSource)
        {
            // Wait for the initiator to be registered by VContainer
            await UniTask.WaitUntil(() => _initiators.ContainsKey(sceneType), cancellationToken: cancellationTokenSource.Token);
            
            if (_initiators.TryGetValue(sceneType, out var initiator))
            {
                await initiator.LoadEntryPoint(enterDataObject, cancellationTokenSource);
            }
            else
            {
                LogService.LogError($"No initiator registered for scene type: {sceneType}");
            }
        }

        public async UniTask InvokeInitiatorExitPoint(SceneType sceneType, CancellationTokenSource cancellationTokenSource)
        {
            if (_initiators.TryGetValue(sceneType, out var initiator))
            {
                await initiator.InitExitPoint(cancellationTokenSource);
            }
            else
            {
                LogService.LogWarning($"No initiator found for scene type: {sceneType}");
            }
        }
    }
}