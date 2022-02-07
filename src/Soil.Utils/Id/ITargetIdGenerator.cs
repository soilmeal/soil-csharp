namespace Soil.Utils.Id;

public interface ITargetIdGenerator<TTarget, TId>
    where TId : IId
{
    TId Generate(TTarget target);
}
