using Darkmatter.Core.Services.CommandFactory.Interfaces;
using UnityEngine;

namespace Darkmatter.Core.Services.CommandFactory.Interfaces
{
    public interface ICommandVoid : IBaseCommand
    {
       void Execute();
    }
}
