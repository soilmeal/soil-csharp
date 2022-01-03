namespace Soil.Buffers;

public partial class UnpooledByteBufferAllocator : ByteBufferAllocator
{
    private readonly UnsafeOp _unsafe;

    public override IByteBufferAllocator.IUnsafeOp Unsafe
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

    public override IByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian)
    {
        var byteBuffer = new UnpooledByteBuffer(this);
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }
}
