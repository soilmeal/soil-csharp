namespace Soil.Buffers;

public partial class UnpooledByteBufferAllocator : IByteBufferAllocator
{
    private readonly UnsafeOp _unsafe;

    public int MaxCapacity
    {
        get
        {
            return Constants.DefaultMaxCapacity;
        }
    }

    public IByteBufferAllocator.IUnsafeOp Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public UnpooledByteBufferAllocator()
    {
        _unsafe = new UnsafeOp(this);
    }

    public IByteBuffer Allocate(
        int capacityHint = Constants.DefaultCapacity,
        Endianless endianless = Endianless.BigEndian)
    {
        var byteBuffer = new UnpooledByteBuffer(this);
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }

    public CompositeByteBuffer CompositeByteBuffer(Endianless endianless = Endianless.BigEndian)
    {
        var byteBuffer = new CompositeByteBuffer(this);
        byteBuffer.Unsafe.Allocate(Constants.CompositionByteBufferCapacityHint, endianless);

        return byteBuffer;
    }
}
