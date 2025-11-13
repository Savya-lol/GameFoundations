using System.Threading;
using Cysharp.Threading.Tasks;
using Darkmatter.Core.Services.SceneService;

namespace Darkmatter.Core.Services.InitiatorService.Scenes
{
    public interface ISceneInitiatorsService
    {
        void RegisterInitiator(ISceneInitiator sceneInitiator);
        void UnregisterInitiator(ISceneInitiator sceneInitiator);
        UniTask InvokeInitiatorExitPoint(SceneType sceneType, CancellationTokenSource cancellationTokenSource);
    }
}