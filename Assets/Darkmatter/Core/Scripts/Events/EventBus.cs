using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Darkmatter.Core.Events
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _map = new();

        public void Publish<T>(T evt)
        {
            if (_map.TryGetValue(typeof(T), out var list))
            {
                // Copy to avoid modification during iteration
                var snapshot = list.ToArray();
                for (int i = 0; i < snapshot.Length; i++)
                    ((Action<T>)snapshot[i])?.Invoke(evt);
            }
        }

        public IDisposable Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (!_map.TryGetValue(t, out var list))
                _map[t] = list = new List<Delegate>();

            list.Add(handler);
            return new Unsub<T>(list, handler);
        }

        private sealed class Unsub<T> : IDisposable
        {
            private readonly List<Delegate> _list;
            private readonly Action<T> _handler;

            public Unsub(List<Delegate> list, Action<T> handler)
            {
                _list = list;
                _handler = handler;
            }

            public void Dispose()
            {
                _list.Remove(_handler);
            }
        }
    }
}

