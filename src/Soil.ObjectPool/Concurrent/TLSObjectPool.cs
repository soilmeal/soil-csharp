using System.Threading;

namespace Soil.ObjectPool.Concurrent;

public class TLSObjectPool<T> : IObjectPool<T>
    where T : class
{
    private readonly IObjectPoolPolicy<T> _policy;

    private readonly int _maximumRetainCountPerThread;

    private readonly ThreadLocal<ObjectPool<T>> _tls;

    public IObjectPoolPolicy<T> Policy
    {
        get
        {
            return _policy;
        }
    }

    public TLSObjectPool(IObjectPoolPolicy<T> policy)
        : this(policy, Constants.DefaultMaxRetainCount)
    {
    }

    public TLSObjectPool(IObjectPoolPolicy<T> policy, int maximumRetainCountPerThread)
    {
        _policy = policy;
        _maximumRetainCountPerThread = maximumRetainCountPerThread;

        _tls = new ThreadLocal<ObjectPool<T>>(Create);
    }

    public T Get()
    {
        return _tls.Value!.Get();
    }

    public void Return(T item)
    {
        _tls.Value!.Return(item);
    }

    private ObjectPool<T> Create()
    {
        return new ObjectPool<T>(_policy, _maximumRetainCountPerThread);
    }
}
