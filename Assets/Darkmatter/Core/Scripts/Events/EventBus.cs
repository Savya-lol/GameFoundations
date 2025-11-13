using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkmatter.Core.Events
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Func<IEvent, UniTask>>> _handlers =
            new Dictionary<Type, List<Func<IEvent, UniTask>>>();

        /// <summary>
        /// Subscribes an event handler to a specific event type.
        /// </summary>
        public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<Func<IEvent, UniTask>>();

            _handlers[eventType].Add(e => handler.HandleAsync((TEvent)e));
        }

        /// <summary>
        /// Publishes an event asynchronously to all registered handlers.
        /// </summary>
        public async UniTask PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var handlers)) return;

            foreach (var handler in handlers)
            {
                await handler(domainEvent);
            }
        }

        /// <summary>
        /// Publishes an event synchronously (fire-and-forget style).
        /// </summary>
        public void Publish<TEvent>(TEvent domainEvent) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var handlers)) return;

            foreach (var handler in handlers)
            {
                // Fire-and-forget UniTask for synchronous publish
                handler(domainEvent).Forget();
            }
        }

        /// <summary>
        /// Clears all registered handlers — useful for testing or resetting scopes.
        /// </summary>
        public void Clear() => _handlers.Clear();
    }
}

