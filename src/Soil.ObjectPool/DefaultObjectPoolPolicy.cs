namespace Soil.ObjectPool;

public class DefaultObjectPoolPolicy<T> : IObjectPoolPolicy<T>
    where T : class, new()
{
    public T Create()
    {
        return new T();
    }

    public bool Return(T item)
    {
        return true;
    }
}
