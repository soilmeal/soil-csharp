using System;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer
{
    private class UnsafeOp : IByteBuffer.IUnsafeOp
    {
        private readonly ByteBuffer _parent;

        private readonly ByteBufferWriter _bufferWriter;

        private readonly IByteBufferAllocator _allocator;

        public IByteBuffer Parent
        {
            get
            {
                return _parent;
            }
        }

        public IByteBufferWriter BufferWriter
        {
            get
            {
                return _bufferWriter;
            }
        }

        public IByteBufferAllocator Allocator
        {
            get
            {
                return _allocator;
            }
        }

        public UnsafeOp(ByteBuffer parent, IByteBufferAllocator allocator)
        {
            _parent = parent;
            _bufferWriter = new ByteBufferWriter(parent);
            _allocator = allocator;
        }

        public void SetReadIndex(int readIndex)
        {
            if (readIndex > _parent._writeIdx)
            {
                throw new InvalidBufferOperationException(
                    InvalidBufferOperationException.ReadIndexExceed);
            }

            _parent._readIdx = readIndex;
        }

        public void SetWriteIndex(int writeIndex)
        {
            if (writeIndex > _parent.Capacity)
            {
                throw new InvalidBufferOperationException(
                    InvalidBufferOperationException.WriteIndexExceed);
            }
            _parent._writeIdx = writeIndex;
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

        public byte[] AsArray()
        {
            return _parent._buffer;
        }

        public Memory<byte> AsMemory()
        {
            return AsArray().AsMemory();
        }

        public Span<byte> AsSpan()
        {
            return AsArray().AsSpan();
        }

        public ReadOnlyMemory<byte> AsReadOnlyMemory()
        {
            return AsMemory();
        }

        public ReadOnlySpan<byte> AsReadOnlySpan()
        {
            return AsSpan();
        }

        public Memory<byte> AsMemoryToSend()
        {
            return AsMemory()[_parent._readIdx.._parent._writeIdx];
        }

        public Memory<byte> AsMemoryToRecv()
        {
            return AsMemory()[_parent._writeIdx.._parent.Capacity];
        }

        public ArraySegment<byte> AsSegmentToSend()
        {
            return new ArraySegment<byte>(AsArray(), _parent._readIdx, _parent.ReadableBytes);
        }

        public ArraySegment<byte> AsSegmentToRecv()
        {
            return new ArraySegment<byte>(AsArray(), _parent._writeIdx, _parent.WritableBytes);
        }

        private class ByteBufferWriter : IByteBufferWriter
        {
            private readonly ByteBuffer _parent;

            public int MaxCapacity
            {
                get
                {
                    return _parent.MaxCapacity;
                }
            }

            public ByteBufferWriter(ByteBuffer parent)
            {
                _parent = parent;
            }

            public void Advance(int count)
            {
                _parent._writeIdx += count;
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                EnsureCapacityIfNeed(sizeHint);

                return _parent.Unsafe.AsMemoryToRecv();
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                EnsureCapacityIfNeed(sizeHint);

                return _parent.Unsafe.AsMemoryToRecv().Span;
            }

            private void EnsureCapacityIfNeed(int sizeHint)
            {
                if (sizeHint <= 0 || sizeHint <= _parent.WritableBytes)
                {
                    return;
                }

                _parent.EnsureCapacity(_parent.WritableBytes - sizeHint);
            }
        }
    }
}
