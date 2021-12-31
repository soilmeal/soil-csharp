namespace Soil.ObjectPool;

public interface IObjectPoolPolicy<T>
    where T : class
{
    T Create();

    bool Return(T item);
}
