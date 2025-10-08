using VContainer;
using Darkmatter.Core.Services.CommandFactory.Interfaces;
namespace Darkmatter.Core.Services.CommandFactory
{
    public abstract class BaseCommand : IBaseCommand
    {
        protected IObjectResolver ObjectResolver;

        public void SetResolver(IObjectResolver objectResolver)
        {
            ObjectResolver = objectResolver;
        }

        public abstract void ResolveDependencies();
    }
}