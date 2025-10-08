using Darkmatter.Core.Services.AudioService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Darkmatter.Core.Scopes
{
    public class CoreLifetimeScope : LifetimeScope
    {
        [SerializeField] private AudioService _audioService;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_audioService);
        }
    }
}
