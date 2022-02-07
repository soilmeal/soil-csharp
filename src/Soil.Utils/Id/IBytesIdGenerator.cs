namespace Soil.Utils.Id;

public interface IBytesIdGenerator<TTarget, TId>
    where TId : IBytesId
{
    public TId Generate(TTarget target);
}
