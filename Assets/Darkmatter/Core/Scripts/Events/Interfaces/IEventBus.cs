using Cysharp.Threading.Tasks;

namespace Darkmatter.Core.Events
{
    public interface IEventBus
    {
        void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent;
        UniTask PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IEvent;
        void Publish<TEvent>(TEvent domainEvent) where TEvent : IEvent;
        void Clear();
    }
}