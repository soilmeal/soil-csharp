namespace Soil.ObjectPool;

public interface IObjectPool<T>
    where T : class
{
    T Get();

    void Return(T item);
}
