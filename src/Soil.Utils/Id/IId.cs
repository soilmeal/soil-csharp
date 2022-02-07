namespace Soil.Utils.Id;

public interface IId
{
    string AsString();
}

public interface IId<TId> : IId
    where TId : struct
{
    TId Value { get; }
}
