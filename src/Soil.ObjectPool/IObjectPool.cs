namespace Soil.ObjectPool;

public interface IObjectPool<T>
    where T : class
{
    IObjectPoolPolicy<T> Policy { get; }

    T Get();

    void Return(T item);
}
