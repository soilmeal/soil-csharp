using System;
using System.Collections.Generic;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer
{
    private class UnsafeOp : IByteBuffer.IUnsafeOp
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

        public Memory<byte> AsMemoryToSend()
        {
            return _parent._buffer.AsMemory()[_parent._readIdx.._parent._writeIdx];
        }

        public Memory<byte> AsMemoryToRecv()
        {
            return _parent._buffer.AsMemory()[_parent._writeIdx.._parent.Capacity];
        }

        public List<ArraySegment<byte>> AsSegmentsToSend()
        {
            return new List<ArraySegment<byte>>()
            {
                new ArraySegment<byte>(
                    _parent._buffer,
                    _parent._readIdx,
                    _parent.ReadableBytes),
            };
        }

        public List<ArraySegment<byte>> AsSegmentsToRecv()
        {
            return new List<ArraySegment<byte>>()
            {
                new ArraySegment<byte>(
                    _parent._buffer,
                    _parent._writeIdx,
                    _parent.WritableBytes),
            };
        }
    }
}
