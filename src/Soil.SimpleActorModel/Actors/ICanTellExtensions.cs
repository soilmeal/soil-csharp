using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public static class ICanTellExtensions
{
    public static Task<object> Ask(this ICanTell target, object? message)
    {
        return target.Ask<object>(message);
    }

    public static Task<T> Ask<T>(this ICanTell target, object? message)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();
        var taskRef = new TaskActorRef<T>();
        taskRef.Initialize(taskCompletionSource);

        target.Tell(message, taskRef);

        return taskCompletionSource.Task;
    }
}
