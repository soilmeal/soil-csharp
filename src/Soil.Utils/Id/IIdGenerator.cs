namespace Soil.Utils.Id;

public interface IIdGenerator<TId>
    where TId : IId
{
    TId Generate();
}
