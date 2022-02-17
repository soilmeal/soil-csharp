using System;

namespace Soil.Utils.Id;

public interface IInt64Id : IId<long>, IEquatable<IInt64Id>
{
    long AsInt64();
}
