using System;

namespace Soil.Utils.Id;

public interface IInt64Id : IId<long>
{
    long AsInt64();
}
