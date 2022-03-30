using System;

namespace Soil.Buffers;

public partial class CompositeByteBuffer
{
    public class UnsafeOp : IByteBuffer.IUnsafeOp
    {
        private readonly CompositeByteBuffer _parent;

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

        public UnsafeOp(CompositeByteBuffer parent, IByteBufferAllocator allocator)
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

        public void Reallocate(int addSizeHint = 0)
        {
            throw new NotSupportedException();
        }

        public void Release()
        {
            _bufferWriter.Release();
        }

        public byte[] AsArray()
        {
            if (_parent._components.Count <= 0)
            {
                return Constants.EmptyBuffer;
            }

            if (_parent._components.Count != 1)
            {
                _parent.Consolidate();
            }

            return _parent._components[0]
                .ByteBuffer
                .Unsafe
                .AsArray();
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
            return new ArraySegment<byte>(AsArray(), _parent._writeIdx, _parent.WritableBytes);
        }

        public ArraySegment<byte> AsSegmentToRecv()
        {
            return new ArraySegment<byte>(AsArray(), _parent._readIdx, _parent.ReadableBytes);
        }

        // TODO: now, consolidate component when send or recv data. consider using this method with list pooling
        // public List<ArraySegment<byte>> AsSegmentsToSend()
        // {
        //     if (_parent._components.Count <= 0)
        //     {
        //         return Constants.EmptySegments;
        //     }

        //     if (_parent._components.Count == 1)
        //     {
        //         Component component = _parent._components[0];
        //         int offset = ComputeOffset(_parent._readIdx, 0, component);
        //         return new List<ArraySegment<byte>>() {
        //             new ArraySegment<byte>(
        //                 component.ByteBuffer.Unsafe.AsArray(),
        //                 offset,
        //                 _parent.ReadableBytes),
        //         };
        //     }

        //     int readIdx = _parent._readIdx;
        //     int readableBytes = _parent.ReadableBytes;
        //     int endOffset = readIdx + readableBytes;
        //     int componentIndex = _parent.IndexOfComponent(readIdx);
        //     List<Component> components = _parent._components;
        //     int bytes = 0;
        //     int count = components.Count;
        //     List<ArraySegment<byte>> result = new(count);
        //     while (componentIndex < count)
        //     {
        //         Component component = components[componentIndex];
        //         if (component.BeginOffset >= endOffset)
        //         {
        //             break;
        //         }

        //         int offset = ComputeOffset(readIdx, bytes, component);
        //         int lengthToGet = MinLengthToGet(readableBytes, bytes, component);
        //         result.Add(new ArraySegment<byte>(
        //             component.ByteBuffer.Unsafe.AsArray(),
        //             offset,
        //             lengthToGet));

        //         bytes += lengthToGet;
        //     }

        //     if (bytes < readableBytes)
        //     {
        //         throw new IndexOutOfRangeException();
        //     }

        //     return result;
        // }

        // // TODO: consider list pooling
        // public List<ArraySegment<byte>> AsSegmentsToRecv()
        // {
        //     if (_parent._components.Count <= 0)
        //     {
        //         return Constants.EmptySegments;
        //     }

        //     if (_parent._components.Count == 1)
        //     {
        //         Component component = _parent._components[0];
        //         int offset = ComputeOffset(_parent._writeIdx, 0, component);
        //         return new List<ArraySegment<byte>>() {
        //             new ArraySegment<byte>(
        //                 component.ByteBuffer.Unsafe.AsArray(),
        //                 offset,
        //                 _parent.WritableBytes),
        //         };
        //     }

        //     int writeIdx = _parent._writeIdx;
        //     int writableBytes = _parent.WritableBytes;
        //     int endOffset = writeIdx + writableBytes;
        //     int componentIndex = _parent.IndexOfComponent(writeIdx);
        //     List<Component> components = _parent._components;
        //     int bytes = 0;
        //     int count = components.Count;
        //     List<ArraySegment<byte>> result = new(count);
        //     while (componentIndex < count)
        //     {
        //         Component component = components[componentIndex];
        //         if (component.BeginOffset >= endOffset)
        //         {
        //             break;
        //         }

        //         int offset = ComputeOffset(writeIdx, bytes, component);
        //         int lengthToGet = MinLengthToGet(writableBytes, bytes, component);
        //         result.Add(new ArraySegment<byte>(
        //             component.ByteBuffer.Unsafe.AsArray(),
        //             offset,
        //             lengthToGet));

        //         bytes += lengthToGet;
        //     }

        //     if (bytes < writableBytes)
        //     {
        //         throw new IndexOutOfRangeException();
        //     }

        //     return result;
        // }

        private class ByteBufferWriter : IByteBufferWriter
        {
            private readonly CompositeByteBuffer _parent;

            private IByteBuffer? _byteBuffer;

            public int MaxCapacity
            {
                get
                {
                    return _parent.MaxCapacity;
                }
            }

            public ByteBufferWriter(CompositeByteBuffer parent)
            {
                _parent = parent;
            }

            public void Advance(int count)
            {
                if (_byteBuffer == null)
                {
                    throw new InvalidOperationException("Call GetMemory or GetSpan first");
                }

                _parent.WriteBytes(_byteBuffer, count);

                ReleaseInnerBuffer();
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                EnsureCapacity(sizeHint);

                return _byteBuffer!.Unsafe.AsMemory();
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                EnsureCapacity(sizeHint);

                return _byteBuffer!.Unsafe.AsSpan();
            }

            public void Release()
            {
                ReleaseInnerBuffer();
            }

            private void ReleaseInnerBuffer()
            {
                if (_byteBuffer == null)
                {
                    return;
                }

                _byteBuffer.Release();
                _byteBuffer = null;
            }

            private void EnsureCapacity(int sizeHint)
            {
                if (sizeHint < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(sizeHint), sizeHint, null);
                }

                if (sizeHint == 0)
                {
                    sizeHint = Constants.DefaultCapacityIncrements;
                }

                _parent.EnsureCapacity(sizeHint);

                _byteBuffer ??= _parent.Allocator.Allocate(sizeHint);
                _byteBuffer.EnsureCapacity(sizeHint);
            }
        }
    }
}
