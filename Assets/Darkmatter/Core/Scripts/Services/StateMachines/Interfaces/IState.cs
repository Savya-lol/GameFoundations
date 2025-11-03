namespace Darkmatter.Core.Services.StateMachines.Interfaces
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void Tick();
    }
}