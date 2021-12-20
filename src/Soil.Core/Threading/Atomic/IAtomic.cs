namespace Soil.Core.Threading.Atomic;

public interface IAtomic<T>
{
    T Read();

    T Exchange(T other);

    T CompareExchange(T other, T comparand);
}
