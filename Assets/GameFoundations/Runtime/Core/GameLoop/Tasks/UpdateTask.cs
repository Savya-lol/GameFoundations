namespace Savya.GameFoundations.GameLoop.Tasks
{
    public class UpdateTask
    {
        public IUpdatable Updatable{ get; }
        public int Priority { get; }
        public UpdateTask(IUpdatable updatable, int priority)
        {
            Updatable = updatable;
            Priority = priority;
        }

        public void Execute()
        {
            Updatable.ManagedUpdate();
        }
    }
}