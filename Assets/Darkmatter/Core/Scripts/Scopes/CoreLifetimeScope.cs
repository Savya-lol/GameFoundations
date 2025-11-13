using Darkmatter.Core.Initiators;
using Darkmatter.Core.Services.AudioService;
using Darkmatter.Core.Services.SceneService;
using Darkmatter.Core.Services.InitiatorService.Scenes;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Darkmatter.Core.Services.LoggingService;
using Darkmatter.Core.Services.API;
using Darkmatter.Core.Events;
using Darkmatter.Core.Services.CommandFactory;
using Darkmatter.Core.Services.CommandFactory.Interfaces;

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
            builder.Register<ICommandFactory, CommandFactory>(Lifetime.Scoped);
            builder.Register<ISceneLoaderService, SceneLoaderService>(Lifetime.Singleton);
            builder.Register<ISceneInitiatorsService, SceneInitiatorsService>(Lifetime.Singleton);
            builder.Register<IApiService, ApiService>(Lifetime.Singleton);
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.RegisterEntryPoint<CoreInitiator>();
        }
    }
}
