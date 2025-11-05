using Darkmatter.Core.Initiators;
using Darkmatter.Core.Services.AudioService;
using Darkmatter.Core.Services.SceneService;
using Darkmatter.Core.Services.InitiatorService.Scenes;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Darkmatter.Core.Services.LoggingService;

namespace Darkmatter.Core.Scopes
{
    public class CoreLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioService _audioService;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_audioService).AsImplementedInterfaces().AsSelf();
            builder.Register<DarkmatterLogger>(Lifetime.Scoped).AsSelf();
            builder.Register<GameInputs>(Lifetime.Singleton).AsSelf();
            builder.Register<ISceneLoaderService, SceneLoaderService>(Lifetime.Singleton);
            builder.Register<ISceneInitiatorsService, SceneInitiatorsService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<CoreInitiator>();
        }
    }
}
