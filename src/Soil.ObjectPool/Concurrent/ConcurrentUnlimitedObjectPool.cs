using System.Collections.Concurrent;

namespace Soil.ObjectPool.Concurrent;

public class ConcurrentUnlimitedObjectPool<T> : IObjectPool<T>
    where T : class
{
    private readonly IObjectPoolPolicy<T> _policy;

    private readonly ConcurrentQueue<T> _queue;

    public IObjectPoolPolicy<T> Policy
    {
        get
        {
            return _policy;
        }
    }

    public ConcurrentUnlimitedObjectPool(IObjectPoolPolicy<T> policy)
    {
        _policy = policy;
        _queue = new ConcurrentQueue<T>();
    }

    public T Get()
    {
        return _queue.TryDequeue(out T? result) ? result : _policy.Create();
    }

    public void Return(T item)
    {
        bool returnAllowed = _policy.Return(item);
        if (!returnAllowed)
        {
            return;
        }

        _queue.Enqueue(item);
    }
}
