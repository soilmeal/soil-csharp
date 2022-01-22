using System;

namespace Soil.Buffers;

public partial class CompositeByteBuffer
{
    private class UnsafeOp : IByteBuffer.IUnsafeOp
    {
        private readonly CompositeByteBuffer _parent;

        private readonly IByteBufferAllocator _allocator;

        public IByteBuffer Parent
        {
            get
            {
                return _parent;
            }
        }

        public IByteBufferAllocator Allocator
        {
            get
            {
                return _allocator;
            }
        }

        public UnsafeOp(CompositeByteBuffer parent, IByteBufferAllocator allocator)
        {
            _parent = parent;
            _allocator = allocator;
        }

        public void SetReadIndex(int readIndex)
        {
            if (readIndex > _parent._writtenIdx)
            {
                throw new InvalidBufferOperationException(
                    InvalidBufferOperationException.ReadIndexExceed);
            }

            _parent._readIdx = readIndex;
        }

        public void SetWrittenIndex(int writtenIndex)
        {
            if (writtenIndex > _parent.Capacity)
            {
                throw new InvalidBufferOperationException(
                    InvalidBufferOperationException.WrittenIndexExceed);
            }
            _parent._writtenIdx = writtenIndex;
        }

        public void Allocate(int _, Endianless endianless)
        {
            _parent._endianless = endianless;
        }

        public void Reallocate()
        {
            throw new NotSupportedException();
        }

        public Memory<byte> AsMemory()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> AsMemoryToRecv()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> AsMemoryToSend()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyMemory<byte> AsReadOnlyMemory()
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> AsReadOnlySpan()
        {
            throw new NotImplementedException();
        }

        public ArraySegment<byte> AsSegmentToRecv()
        {
            throw new NotImplementedException();
        }

        public ArraySegment<byte> AsSegmentToSend()
        {
            throw new NotImplementedException();
        }

        public Span<byte> AsSpan()
        {
            throw new NotImplementedException();
        }

        public Span<byte> AsSpanToRecv()
        {
            throw new NotImplementedException();
        }

        public Span<byte> AsSpanToSend()
        {
            throw new NotImplementedException();
        }
    }
}
