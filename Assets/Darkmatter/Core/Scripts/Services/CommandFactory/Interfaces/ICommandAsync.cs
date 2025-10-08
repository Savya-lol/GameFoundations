using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkmatter.Core.Services.CommandFactory.Interfaces
{
    public interface ICommandAsync : IBaseCommand
    {
        UniTask Execute(CancellationTokenSource cancellationTokenSource);
    }
}
