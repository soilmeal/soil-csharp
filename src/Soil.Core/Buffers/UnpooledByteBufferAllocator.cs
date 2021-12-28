namespace Soil.Core.Buffers;

public partial class UnpooledByteBufferAllocator : ByteBufferAllocator
{
    private static readonly UnpooledByteBufferAllocator _instance = new();

    public static UnpooledByteBufferAllocator Instance
    {
        get
        {
            return _instance;
        }
    }

    private readonly UnsafeOp _unsafe;

    public override IByteBufferAllocator.IUnsafeOp<ByteBuffer> Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    private UnpooledByteBufferAllocator()
    {
        _unsafe = new UnsafeOp(this);
    }

    public override ByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian)
    {
        var byteBuffer = new UnpooledByteBuffer(this);
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }
}
