using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.SceneService;

namespace Darkmatter.Core.Services.InitiatorService.Scenes
{
    public class SceneInitiatorsService : ISceneInitiatorsService
    {
        private readonly Dictionary<SceneType, ISceneInitiator> _sceneInitiators = new Dictionary<SceneType, ISceneInitiator>();

        public void RegisterInitiator(ISceneInitiator sceneInitiator)
        {
            _sceneInitiators.Add(sceneInitiator.SceneType, sceneInitiator);
        }

        public void UnregisterInitiator(ISceneInitiator sceneInitiator)
        {
            _sceneInitiators.Remove(sceneInitiator.SceneType);
        }

        public async UniTask InvokeInitiatorExitPoint(SceneType sceneType, CancellationTokenSource cancellationTokenSource)
        {
            await _sceneInitiators[sceneType].InitExitPoint(cancellationTokenSource);
        }
    }
}