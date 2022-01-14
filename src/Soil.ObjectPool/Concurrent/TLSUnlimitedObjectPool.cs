using System.Threading;

namespace Soil.ObjectPool.Concurrent;

public class TLSUnlimitedObjectPool<T> : IObjectPool<T>
    where T : class
{
    private readonly IObjectPoolPolicy<T> _policy;

    private readonly ThreadLocal<UnlimitedObjectPool<T>> _tls;

    public IObjectPoolPolicy<T> Policy
    {
        get
        {
            return _policy;
        }
    }

    public TLSUnlimitedObjectPool(IObjectPoolPolicy<T> policy)
    {
        _policy = policy;
        _tls = new ThreadLocal<UnlimitedObjectPool<T>>(Create);
    }

    public T Get()
    {
        return _tls.Value!.Get();
    }

    public void Return(T item)
    {
        _tls.Value!.Return(item);
    }

    private UnlimitedObjectPool<T> Create()
    {
        return new UnlimitedObjectPool<T>(_policy);
    }
}
