using System;

namespace Soil.Utils.Id;

public interface IId
{
    string AsString();
}

public interface IId<TId> : IId, IEquatable<TId>
    where TId : struct
{
    TId Value { get; }
}
