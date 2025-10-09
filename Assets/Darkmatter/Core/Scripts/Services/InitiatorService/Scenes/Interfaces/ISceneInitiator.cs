using System.Threading;
using Darkmatter.Core.Services.SceneService;
using UnityEngine;

namespace Darkmatter.Core.Services.InitiatorService.Scenes
{
    public interface ISceneInitiator
    {
        SceneType SceneType { get; }
        Awaitable LoadEntryPoint(IInitiatorEnterData enterDataObject, CancellationTokenSource cancellationTokenSource);
        Awaitable StartEntryPoint(IInitiatorEnterData enterDataObject, CancellationTokenSource cancellationTokenSource);
        Awaitable InitExitPoint(CancellationTokenSource cancellationTokenSource);
    }
}
