using UnityEngine;

namespace Darkmatter.Core.Services.StateMachines.Interfaces
{
    public interface IBaseStateMachine
    {
        public IState Current { get; }
        void Push(IState state);
        void Pop();
        void Replace(IState state);
        void Tick();
    }
}
