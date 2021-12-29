using System;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Buffers;

public abstract partial class ByteBuffer : IByteBuffer<ByteBuffer>
{
    public class UnsafeOp
    {
        private readonly ILogger<UnsafeOp> _logger;

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

        public UnsafeOp(
            ByteBuffer parent,
            ByteBufferAllocator allocator,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UnsafeOp>();

            _parent = parent;
            _allocator = allocator;
        }

        public Memory<byte> Memory()
        {
            return _parent._buffer;
        }

        public void SetReadIndex(int readIndex)
        {
            _parent._readIdx = readIndex;
        }

        public void SetWrittenIndex(int writtenIndex)
        {
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
                throw new InvalidBufferOperation(InvalidBufferOperation.ReleaseTwice);
            }

            _parent.Clear();

            byte[] buffer = _parent._buffer;
            _parent._buffer = _defaultBuffer;
            _parent._endianless = Endianless.None;

            return buffer;
        }
    }
}
