using System;
using Soil.Threading.Atomic;

namespace Soil.Game;

public class Object
{
    private static readonly AtomicInt64 _idGenerator = new(-1L);

    private readonly long _id = _idGenerator.Increment();

    public long Id
    {
        get
        {
            return _id;
        }
    }

    public static void Destroy(Object? obj)
    {
        if (obj == null)
        {
            return;
        }

        if (obj is GameObject gameObject)
        {
            gameObject.World.Destroy(gameObject);
            return;
        }

        if (obj is Component component)
        {
            component.GameObject?.Destroy(component);
        }

        throw new ArgumentException("only support class derived \"GameObject\" or \"Component\"");
    }

    public Object()
    {
    }
}
