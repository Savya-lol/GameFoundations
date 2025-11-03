using System.Collections.Generic;
using Darkmatter.Core.Services.StateMachines.Interfaces;

namespace Darkmatter.Core.Services.StateMachines
{
    public class BaseStateMachine
    {
        private readonly Stack<IState> _stack = new();
        public IState Current => _stack.Count > 0 ? _stack.Peek() : null;

        public void Push(IState s)
        {
            Current?.OnExit();
            _stack.Push(s);
            s.OnEnter();
        }
        public void Replace(IState s)
        {
            if (Current != null)
            {
                Current.OnExit();
                _stack.Pop();
            }
            _stack.Push(s);
            s.OnEnter();
        }
        public void Pop()
        {
            if (Current == null) return;
            Current.OnExit();
            _stack.Pop();
            _stack.Peek()?.OnEnter();
        }
        public void Tick()
        {
            Current?.Tick();
        }
    }
}