namespace Soil.Core.Threading.Tasks;

public abstract class TaskScheduler : System.Threading.Tasks.TaskScheduler
{
    public abstract IThreadFactory ThreadFactory { get; }
}
