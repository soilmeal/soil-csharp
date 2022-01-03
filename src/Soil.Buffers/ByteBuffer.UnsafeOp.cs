using System;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer
{
    public class UnsafeOp
    {
        private readonly ByteBuffer _parent;

        private readonly ByteBufferAllocator _allocator;

        public ByteBuffer Parent
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

        public Memory<byte> AsMemoryToSend()
        {
            return _parent._buffer.AsMemory()[^_parent._writtenIdx..];
        }

        public Memory<byte> AsMemoryToRecv()
        {
            return _parent._buffer.AsMemory()[^_parent._readIdx..];
        }

        public ArraySegment<byte> AsSegmentToSend()
        {
            return new ArraySegment<byte>(_parent._buffer, 0, _parent._writtenIdx);
        }

        public ArraySegment<byte> AsSegmentToRecv()
        {
            return new ArraySegment<byte>(_parent._buffer, 0, _parent._readIdx);
        }
    }
}
