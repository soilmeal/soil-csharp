namespace Soil.ObjectPool;

public class ObjectPool<T> : ISizedObjectPool<T>
    where T : class
{
    private readonly IObjectPoolPolicy<T> _policy;

    private readonly int _maximumRetainCount;

    private readonly ObjectNode[] _nodes;

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

    public ObjectPool(IObjectPoolPolicy<T> policy)
        : this(policy, Constants.DefaultMaxRetainCount)
    {
    }

    public ObjectPool(IObjectPoolPolicy<T> policy, int maximumRetainCount)
    {
        _policy = policy;
        _maximumRetainCount = maximumRetainCount;
        _nodes = new ObjectNode[maximumRetainCount];
    }

    public T Get()
    {
        for (int i = 0; i < _maximumRetainCount; ++i)
        {
            ObjectNode node = _nodes[i];
            if (node.Item == null)
            {
                continue;
            }

            T result = node.Item;
            node.Item = null;

            return result;
        }

        return _policy.Create();
    }

    public void Return(T item)
    {
        bool returnAllowed = _policy.Return(item);
        if (!returnAllowed)
        {
            return;
        }

        for (int i = 0; i < _maximumRetainCount; ++i)
        {
            ObjectNode node = _nodes[i];
            if (node.Item != null)
            {
                continue;
            }

            node.Item = item;
            return;
        }
    }

    private struct ObjectNode
    {
        public T? Item;
    }
}
