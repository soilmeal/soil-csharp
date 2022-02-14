using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Message.System;

public class Stop : SystemMessage
{
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public TaskCompletionSource<bool> TaskCompletionSource
    {
        get
        {
            return _taskCompletionSource;
        }
    }

    public Task<bool> Task
    {
        get
        {
            return _taskCompletionSource.Task;
        }
    }

    private Stop()
    {
    }

    public static Stop Create()
    {
        return new Stop();
    }
}
