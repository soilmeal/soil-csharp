using System;
using System.Threading;

namespace Soil.ObjectPool;

public class LimitedObjectPool<T> : IObjectPool<T>
    where T : class
{
    private const int DefaultMaxRetainedCount = 100;

    private const int DefaultIntialCreationCount = 0;

    private readonly IObjectPoolPolicy<T> _policy;

    private readonly int _maximumRetainedCount;

    private readonly int _initialCreationCount;

    private readonly ObjectNode[] _items;

    private T? _firstItem;


    public IObjectPoolPolicy<T> Policy
    {
        get
        {
            return _policy;
        }
    }

    public int MaximumRetainedCount
    {
        get
        {
            return _maximumRetainedCount;
        }
    }

    public int InitialCreationCount
    {
        get
        {
            return _initialCreationCount;
        }
    }

    public LimitedObjectPool(IObjectPoolPolicy<T> policy)
        : this(policy, DefaultMaxRetainedCount)
    {

    }

    public LimitedObjectPool(IObjectPoolPolicy<T> policy, int maximumRetainedCount)
        : this(policy, maximumRetainedCount, DefaultIntialCreationCount)
    {

    }

    public LimitedObjectPool(
        IObjectPoolPolicy<T> policy,
        int maximumRetainedCount,
        int initialCreationCount)
    {
        _policy = policy;
        _maximumRetainedCount = maximumRetainedCount;
        _initialCreationCount = Math.Min(initialCreationCount, _maximumRetainedCount);

        _items = new ObjectNode[maximumRetainedCount - 1];
        if (initialCreationCount <= 0)
        {
            return;
        }

        _firstItem = _policy.Create();
        if (initialCreationCount == 1)
        {
            return;
        }

        for (int i = 0; i < initialCreationCount - 1; ++i)
        {
            _items[i].Item = _policy.Create();
        }
    }

    public T Get()
    {
        T? item = _firstItem;
        if (item != null && Interlocked.CompareExchange(ref _firstItem, null, item) == item)
        {
            return item;
        }

        ObjectNode[] items = _items;
        int itemsLen = items.Length;
        for (int i = 0; i < itemsLen; ++i)
        {
            ObjectNode node = items[i];
            item = node.Item;
            if (item != null
                && Interlocked.CompareExchange(ref node.Item, null, item) == item)
            {
                return item;
            }
        }

        item = _policy.Create();
        return item;
    }

    public void Return(T item)
    {
        bool returnAllowed = _policy.Return(item);
        if (!returnAllowed)
        {
            return;
        }

        if (_firstItem == null && Interlocked.CompareExchange(ref _firstItem, item, null) == null)
        {
            return;
        }

        ObjectNode[] items = _items;
        int itemsLen = items.Length;
        for (int i = 0; i < itemsLen; ++i)
        {
            ObjectNode node = items[i];
            if (Interlocked.CompareExchange(ref node.Item, item, null) == null)
            {
                return;
            }
        }
    }

    private struct ObjectNode
    {
        public T? Item;
    }
}
