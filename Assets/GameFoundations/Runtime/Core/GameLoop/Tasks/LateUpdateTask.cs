namespace Savya.GameFoundations.GameLoop.Tasks
{
    public class LateUpdateTask
    {
        public ILateUpdatable LateUpdatable { get; }
        public int Priority { get; }
        public LateUpdateTask(ILateUpdatable lateUpdatable, int priority)
        {
            LateUpdatable = lateUpdatable;
            Priority = priority;
        }

        public void Execute()
        {
            LateUpdatable.ManagedLateUpdate();
        }
    }
}