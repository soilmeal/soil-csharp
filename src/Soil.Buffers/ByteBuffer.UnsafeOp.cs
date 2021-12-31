using System;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer<ByteBuffer>
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

        public IByteBufferAllocator<ByteBuffer> Allocator
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

        public void Reallocate(int capacityHint)
        {
            _parent._buffer = _allocator.Unsafe.Reallocate(_parent._buffer, capacityHint);
        }

        public byte[] Release()
        {
            if (!_parent.IsInitialized)
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.ReleaseTwice);
            }

            _parent.Clear();

            byte[] buffer = _parent._buffer;
            _parent._buffer = _defaultBuffer;
            _parent._endianless = Endianless.None;

            return buffer;
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
