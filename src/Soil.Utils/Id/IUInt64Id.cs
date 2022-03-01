using System;

namespace Soil.Utils.Id;

public interface IUInt64Id : IId<ulong>, IEquatable<IUInt64Id>
{
    ulong AsUInt64();
}
