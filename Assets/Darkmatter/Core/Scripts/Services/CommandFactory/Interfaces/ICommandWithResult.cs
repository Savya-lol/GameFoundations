using UnityEngine;

namespace Darkmatter.Core.Services.CommandFactory.Interfaces
{
    public interface ICommandWithResult<TReturn> : IBaseCommand
    {
        TReturn Execute();
    }
}
