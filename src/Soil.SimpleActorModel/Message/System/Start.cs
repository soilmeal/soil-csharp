using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Message.System;

internal class Start : SystemMessage
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

    private Start()
    {
    }

    public static Start Create()
    {
        return new Start();
    }
}
