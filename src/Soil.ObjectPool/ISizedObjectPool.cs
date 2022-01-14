namespace Soil.ObjectPool;

public interface ISizedObjectPool<T> : IObjectPool<T>
    where T : class
{
    int MaximumRetainCount { get; }
}
