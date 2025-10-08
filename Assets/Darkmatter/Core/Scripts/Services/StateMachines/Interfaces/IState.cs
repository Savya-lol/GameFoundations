namespace Darkmatter.Core.Services.StateMachines.Interfaces
{
    public interface IState
    {
        bool CanInterrupt { get; }
        void OnEnter();
        void OnExit();
        void Tick();
    }
}