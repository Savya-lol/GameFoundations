using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkmatter.Core.Services.CommandFactory.Interfaces
{
    public interface ICommandAsyncWithResult<TReturn> : IBaseCommand
    {
        UniTask<TReturn> Execute(CancellationTokenSource cancellationTokenSource);
    }
}
