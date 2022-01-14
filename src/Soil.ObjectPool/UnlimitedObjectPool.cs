using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Soil.ObjectPool;

public class UnlimitedObjectPool<T> : IObjectPool<T>
    where T : class
{
    private readonly IObjectPoolPolicy<T> _policy;

    private readonly Queue<T> _queue;

    public IObjectPoolPolicy<T> Policy
    {
        get
        {
            return _policy;
        }
    }

    public UnlimitedObjectPool(IObjectPoolPolicy<T> policy)
    {
        _policy = policy;
        _queue = new Queue<T>();
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
