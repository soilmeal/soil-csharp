namespace Soil.Core.Threading.Atomic;

public interface IAtomicNumeric<TNumeric> : IAtomic<TNumeric>
    where TNumeric : struct
{
    TNumeric Add(TNumeric other);

    TNumeric Increment();

    TNumeric Decrement();
}
