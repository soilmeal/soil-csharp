namespace Soil.Core.Threading.Tasks;

public abstract class TaskScheduler : System.Threading.Tasks.TaskScheduler
{
    public virtual IThreadFactory ThreadFactory => FormattedNameThreadFactory.Default;
}
