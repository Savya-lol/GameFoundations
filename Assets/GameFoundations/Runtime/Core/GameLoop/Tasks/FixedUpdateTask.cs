namespace Savya.GameFoundations.GameLoop.Tasks
{
    public class FixedUpdateTask
    {
        public IFixedUpdatable FixedUpdatable { get; }
        public int Priority { get; }
        public FixedUpdateTask(IFixedUpdatable fixedUpdatable, int priority)
        {
            FixedUpdatable = fixedUpdatable;
            Priority = priority;
        }

        public void Execute()
        {
            FixedUpdatable.ManagedFixedUpdate();
        }
    }
}