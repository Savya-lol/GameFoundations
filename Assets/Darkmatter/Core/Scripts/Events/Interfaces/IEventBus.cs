using System;

namespace Darkmatter.Core.Events
{
    public interface IEventBus
    {
        /// <summary>
        /// Publish an event of type T to all subscribers.
        /// </summary>
        void Publish<T>(T evt);

        /// <summary>
        /// Subscribe to an event of type T.
        /// Returns IDisposable – calling Dispose() unsubscribes.
        /// </summary>
        IDisposable Subscribe<T>(Action<T> handler);
    }
}