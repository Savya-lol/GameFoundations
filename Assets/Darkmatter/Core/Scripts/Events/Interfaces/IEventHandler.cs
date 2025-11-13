using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkmatter.Core.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        UniTask HandleAsync(TEvent domainEvent);
    }
}
