using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Soil.Core.Threading.Atomic;

public class AtomicBool : IAtomic<bool>
{
    private int _value;

    public AtomicBool()
        : this(false)
    {

    }

    public AtomicBool(bool value)
    {
        _value = Convert.ToInt32(value);
    }

    public bool Read()
    {
        return Convert.ToBoolean(_value);
    }

    public bool Exchange(bool other)
    {
        int otherInt = Convert.ToInt32(other);
        int result = Interlocked.Exchange(ref _value, otherInt);
        return Convert.ToBoolean(result);
    }

    public bool CompareExchange(bool other, bool comparand)
    {
        int otherInt = Convert.ToInt32(other);
        int comparandInt = Convert.ToInt32(comparand);
        int result = Interlocked.CompareExchange(ref _value, otherInt, comparandInt);
        return Convert.ToBoolean(result);
    }
}
