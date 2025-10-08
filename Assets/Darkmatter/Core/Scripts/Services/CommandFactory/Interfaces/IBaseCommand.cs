using VContainer;

namespace Darkmatter.Core.Services.CommandFactory.Interfaces
{
    public interface IBaseCommand
    {
        void SetResolver(IObjectResolver objectResolver);
        void ResolveDependencies();
    }
}