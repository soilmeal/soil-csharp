using System;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer
{
    public class UnsafeOp : IByteBuffer.IUnsafeOp
    {
        private readonly ByteBuffer _parent;

        private readonly ByteBufferAllocator _allocator;

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

        public UnsafeOp(ByteBuffer parent, ByteBufferAllocator allocator)
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

        public void Allocate(int capacityHint, Endianless endianless)
        {
            _parent._buffer = _allocator.Unsafe.Allocate(capacityHint);
            _parent._endianless = endianless;
        }

        public void Reallocate()
        {
            _parent._buffer = _allocator.Unsafe.Reallocate(_parent._buffer);
        }

        public Span<byte> AsSpanToSend()
        {
            return _parent._buffer.AsSpan()[_parent._readIdx.._parent._writtenIdx];
        }

        public Span<byte> AsSpanToRecv()
        {
            return _parent._buffer.AsSpan()[_parent._writtenIdx.._parent.Capacity];
        }

        public Memory<byte> AsMemoryToSend()
        {
            return _parent._buffer.AsMemory()[_parent._readIdx.._parent._writtenIdx];
        }

        public Memory<byte> AsMemoryToRecv()
        {
            return _parent._buffer.AsMemory()[_parent._writtenIdx.._parent.Capacity];
        }

        public ArraySegment<byte> AsSegmentToSend()
        {
            return new ArraySegment<byte>(
                _parent._buffer,
                _parent._readIdx,
                _parent.ReadableBytes);
        }

        public ArraySegment<byte> AsSegmentToRecv()
        {
            return new ArraySegment<byte>(
                _parent._buffer,
                _parent._writtenIdx,
                _parent.WritableBytes);
        }

        public Span<byte> AsSpan()
        {
            return _parent._buffer.AsSpan();
        }

        public Memory<byte> AsMemory()
        {
            return _parent._buffer.AsMemory();
        }

        public ReadOnlySpan<byte> AsReadOnlySpan()
        {
            return _parent._buffer.AsSpan();
        }

        public ReadOnlyMemory<byte> AsReadOnlyMemory()
        {
            return _parent._buffer.AsMemory();
        }
    }
}
