namespace Soil.Threading.Atomic;

public interface IAtomicInteger<TInteger> : IAtomicNumeric<TInteger>
    where TInteger : struct
{
    public TInteger And(TInteger other);

    public TInteger Or(TInteger other);
}
