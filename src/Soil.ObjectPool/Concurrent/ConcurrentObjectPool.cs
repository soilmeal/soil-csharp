using System;
using System.Threading;

namespace Soil.ObjectPool.Concurrent;

public class ConcurrentObjectPool<T> : IObjectPool<T>
    where T : class
{

    private readonly IObjectPoolPolicy<T> _policy;

    private readonly int _maximumRetainCount;

    private readonly ObjectNode[] _nodes;

    private readonly int _itemsLen;

    private T? _firstItem;


    public IObjectPoolPolicy<T> Policy
    {
        get
        {
            return _policy;
        }
    }

    public int MaximumRetainCount
    {
        get
        {
            return _maximumRetainCount;
        }
    }

    public ConcurrentObjectPool(IObjectPoolPolicy<T> policy)
        : this(policy, Constants.DefaultMaxRetainCount)
    {

    }

    public ConcurrentObjectPool(IObjectPoolPolicy<T> policy, int maximumRetainedCount)
    {
        _policy = policy;
        _maximumRetainCount = maximumRetainedCount;

        _nodes = new ObjectNode[maximumRetainedCount - 1];
        _itemsLen = _nodes.Length;
        _firstItem = null;
    }

    public T Get()
    {
        T? item = _firstItem;
        if (item != null && Interlocked.CompareExchange(ref _firstItem, null, item) == item)
        {
            return item;
        }

        ObjectNode[] nodes = _nodes;
        for (int i = 0; i < _itemsLen; ++i)
        {
            ObjectNode node = nodes[i];
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

        ObjectNode[] items = _nodes;
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
