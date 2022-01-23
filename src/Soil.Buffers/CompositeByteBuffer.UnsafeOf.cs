using System;
using System.Collections.Generic;

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
            throw new NotSupportedException();
        }

        public Memory<byte> AsMemoryToSend()
        {
            throw new NotSupportedException();
        }

        public Memory<byte> AsMemoryToRecv()
        {
            throw new NotSupportedException();
        }

        public List<ArraySegment<byte>> AsSegmentsToRecv()
        {
            throw new NotSupportedException();
        }

        public List<ArraySegment<byte>> AsSegmentsToSend()
        {
            throw new NotSupportedException();
        }
    }
}
