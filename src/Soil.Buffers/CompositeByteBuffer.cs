using System;
using System.Collections.Generic;
using System.Linq;

namespace Soil.Buffers;

public partial class CompositeByteBuffer : IByteBuffer
{
    private Endianless _endianless;

    private readonly List<Component> _components = new();

    private readonly LastAccessedComponent _lastAccessed = new();

    private int _readIdx = 0;

    private int _writtenIdx = 0;

    private readonly UnsafeOp _unsafe;

    public int WrittenIndex
    {
        get
        {
            return _writtenIdx;
        }
    }

    public int WritableBytes
    {
        get
        {
            return Capacity - _writtenIdx;
        }
    }

    public int Capacity
    {
        get
        {
            int count = _components.Count;
            return count > 0 ? _components[count - 1].EndOffset : 0;
        }
    }

    public int MaxCapacity
    {
        get
        {
            return Constants.MaxCapacity;
        }
    }

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _unsafe.Allocator;
        }
    }

    public IByteBuffer.IUnsafeOp Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public bool IsInitialized
    {
        get
        {
            return _endianless != Endianless.None;
        }
    }

    public int ReadIndex
    {
        get
        {
            return _readIdx;
        }
    }

    public int ReadableBytes
    {
        get
        {
            return _writtenIdx - _readIdx;
        }
    }

    public Endianless Endianless
    {
        get
        {
            return _endianless;
        }
    }

    public IReadOnlyByteBuffer.IReadOnlyUnsafeOp ReadOnlyUnsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public CompositeByteBuffer(IByteBufferAllocator allocator)
    {
        _unsafe = new UnsafeOp(this, allocator);

        _readIdx = 0;
        _writtenIdx = 0;
    }

    public bool Readable()
    {
        return ReadableBytes > 0;
    }

    public bool Readable(int length)
    {
        return ReadableBytes >= length;
    }

    public bool Writable()
    {
        return WritableBytes > 0;
    }

    public bool Writable(int length)
    {
        return WritableBytes >= length;
    }

    public CompositeByteBuffer AddComponent(IByteBuffer byteBuffer)
    {
        return AddComponent(false, _components.Count, byteBuffer);
    }

    public CompositeByteBuffer AddComponent(bool increaseWrittenIndex, IByteBuffer byteBuffer)
    {
        return AddComponent(increaseWrittenIndex, _components.Count, byteBuffer);
    }

    public CompositeByteBuffer AddComponent(int componentIndex, IByteBuffer byteBuffer)
    {
        return AddComponent(false, componentIndex, byteBuffer);
    }

    public CompositeByteBuffer AddComponent(
        bool increaseWrittenIndex,
        int componentIndex,
        IByteBuffer byteBuffer)
    {
        ThrowIfComponentIndexOutOfRange(componentIndex);

        ThrowIfInvalidByteBuffer(byteBuffer);

        bool wasAdded = false;
        try
        {
            int readableBytes = byteBuffer.ReadableBytes;
            if (unchecked(Capacity + readableBytes) < 0)
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.MaxCapacityReached);
            }

            Component component = new Component(byteBuffer);
            _components.Insert(componentIndex, component);
            if (componentIndex > 0)
            {
                component.AdjustOffset(_components[componentIndex - 1]);
            }

            if (componentIndex < (_components.Count - 1))
            {
                UpdateComponentsOffset(componentIndex);
            }

            if (increaseWrittenIndex)
            {
                _writtenIdx += readableBytes;
            }

            return this;
        }
        finally
        {
            if (!wasAdded)
            {
                byteBuffer.Release();
            }
        }
    }

    public CompositeByteBuffer AddComponents(params IByteBuffer[] byteBuffers)
    {
        return AddComponents(false, _components.Count, byteBuffers);
    }

    public CompositeByteBuffer AddComponents(
        bool increaseWrittenIndex,
        params IByteBuffer[] byteBuffers)
    {
        return AddComponents(increaseWrittenIndex, _components.Count, byteBuffers);
    }

    public CompositeByteBuffer AddComponents(int componentIndex, params IByteBuffer[] byteBuffers)
    {
        return AddComponents(false, componentIndex, byteBuffers);
    }

    public CompositeByteBuffer AddComponents(
        bool increaseWrittenIndex,
        int componentIndex,
        params IByteBuffer[] byteBuffers)
    {
        return AddComponents(increaseWrittenIndex, componentIndex, byteBuffers.AsEnumerable());
    }

    public CompositeByteBuffer AddComponents(IEnumerable<IByteBuffer> byteBuffers)
    {
        return AddComponents(false, _components.Count, byteBuffers);
    }

    public CompositeByteBuffer AddComponents(
        bool increaseWrittenIndex,
        IEnumerable<IByteBuffer> byteBuffers)
    {
        return AddComponents(increaseWrittenIndex, _components.Count, byteBuffers);
    }

    public CompositeByteBuffer AddComponents(
        int componentIndex,
        IEnumerable<IByteBuffer> byteBuffers)
    {
        return AddComponents(false, componentIndex, byteBuffers);
    }

    public CompositeByteBuffer AddComponents(
        bool increaseWrittenIndex,
        int componentIndex,
        IEnumerable<IByteBuffer> byteBuffers)
    {
        ThrowIfComponentIndexOutOfRange(componentIndex);

        foreach (IByteBuffer byteBuffer in byteBuffers)
        {
            AddComponent(increaseWrittenIndex, componentIndex, byteBuffer);
        }

        return this;
    }

    public void Consolidate()
    {
        IByteBuffer newByteBuffer = Allocator.Allocate(Capacity, _endianless);
        GetBytes(0, newByteBuffer);

        Component consolidated = new Component(newByteBuffer);
        _components.Clear();
        _components.Add(consolidated);
    }

    public void EnsureCapacity()
    {
        EnsureCapacity(Constants.DefaultCapacityIncrements);
    }

    public void EnsureCapacity(int length)
    {
        if (Writable(length))
        {
            return;
        }

        IByteBuffer byteBuffer = Allocator.Allocate(length, _endianless);
        byteBuffer.Unsafe.SetReadIndex(0);
        byteBuffer.Unsafe.SetWrittenIndex(length);
        AddComponent(false, byteBuffer);
    }

    public int GetBytes(int index, byte[] dest)
    {
        return GetBytes(index, dest, Math.Min(Capacity - index, dest.Length));
    }

    public int GetBytes(int index, byte[] dest, int length)
    {
        return GetBytes(index, dest, 0, length);
    }

    public int GetBytes(int index, byte[] dest, int destIndex, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        int componentIndex = IndexOfComponent(index);
        int gotBytes = 0;
        int count = _components.Count;
        do
        {
            Component component = _components[componentIndex];
            int offset = index + gotBytes - component.BeginOffset;
            int destOffset = destIndex + gotBytes;
            int len = Math.Min(length - gotBytes, component.Length);
            gotBytes += component.ByteBuffer.GetBytes(offset, dest, destOffset, len);
            componentIndex += 1;
        } while (gotBytes <= length || componentIndex < count);

        if (gotBytes < length)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed);
        }

        return length;
    }

    public int GetBytes(int index, Span<byte> dest)
    {
        return GetBytes(index, dest, Math.Min(Capacity - index, dest.Length));
    }

    public int GetBytes(int index, Span<byte> dest, int length)
    {
        return GetBytes(index, dest, 0, length);
    }

    public int GetBytes(int index, Span<byte> dest, int destIndex, int length)
    {
        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        int componentIndex = IndexOfComponent(index);
        int gotBytes = 0;
        int count = _components.Count;
        do
        {
            Component component = _components[componentIndex];
            int offset = index + gotBytes - component.BeginOffset;
            int destOffset = destIndex + gotBytes;
            int len = Math.Min(length - gotBytes, component.Length);
            gotBytes += component.ByteBuffer.GetBytes(offset, dest, destOffset, len);
            componentIndex += 1;
        } while (gotBytes <= length || componentIndex < count);

        if (gotBytes < length)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed);
        }

        return length;
    }

    public int GetBytes(int index, Memory<byte> dest)
    {
        throw new NotImplementedException();
    }

    public int GetBytes(int index, Memory<byte> dest, int length)
    {
        throw new NotImplementedException();
    }

    public int GetBytes(int index, Memory<byte> dest, int destIndex, int length)
    {
        throw new NotImplementedException();
    }

    public byte ReadByte()
    {
        throw new NotImplementedException();
    }

    public Memory<byte> ReadBytes(int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(byte[] dest)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(byte[] dest, int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(byte[] dest, int destIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(Span<byte> dest)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(Span<byte> dest, int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(Span<byte> dest, int destIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(Memory<byte> dest)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(Memory<byte> dest, int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(Memory<byte> dest, int destIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(IByteBuffer dest)
    {
        throw new NotImplementedException();
    }

    public int ReadBytes(IByteBuffer dest, int length)
    {
        throw new NotImplementedException();
    }

    public char ReadChar()
    {
        throw new NotImplementedException();
    }

    public char ReadChar(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public double ReadDouble()
    {
        throw new NotImplementedException();
    }

    public double ReadDouble(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public short ReadInt16()
    {
        throw new NotImplementedException();
    }

    public short ReadInt16(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public int ReadInt32()
    {
        throw new NotImplementedException();
    }

    public int ReadInt32(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public long ReadInt64()
    {
        throw new NotImplementedException();
    }

    public long ReadInt64(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public sbyte ReadSByte()
    {
        throw new NotImplementedException();
    }

    public float ReadSingle()
    {
        throw new NotImplementedException();
    }

    public float ReadSingle(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public ushort ReadUInt16()
    {
        throw new NotImplementedException();
    }

    public ushort ReadUInt16(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public uint ReadUInt32()
    {
        throw new NotImplementedException();
    }

    public uint ReadUInt32(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public ulong ReadUInt64()
    {
        throw new NotImplementedException();
    }

    public ulong ReadUInt64(Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public void Release()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.AlreadyReleased);
        }

        Clear();

        _lastAccessed.Reset();

        foreach (var component in _components)
        {
            component.Release();
        }

        _endianless = Endianless.None;
    }

    public void ResetReadIndex()
    {
        throw new NotImplementedException();
    }

    public void ResetWrittenIndex()
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, byte[] src)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, byte[] src, int length)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, byte[] src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, ReadOnlySpan<byte> src)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, ReadOnlySpan<byte> src, int length)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, ReadOnlySpan<byte> src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, ReadOnlyMemory<byte> src)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, ReadOnlyMemory<byte> src, int length)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, ReadOnlyMemory<byte> src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteByte(byte value)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(byte[] src)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(byte[] src, int length)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(byte[] src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(ReadOnlySpan<byte> src)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(ReadOnlySpan<byte> src, int length)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(ReadOnlySpan<byte> src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(ReadOnlyMemory<byte> src)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(ReadOnlyMemory<byte> src, int length)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(ReadOnlyMemory<byte> src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(IByteBuffer src)
    {
        throw new NotImplementedException();
    }

    public int WriteBytes(IByteBuffer src, int length)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteChar(char value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteChar(char value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteDouble(double value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteDouble(double value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteInt16(short value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteInt16(short value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteInt32(int value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteInt32(int value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteInt64(long value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteInt64(long value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteSByte(sbyte value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteSingle(float value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteSingle(float value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteUInt16(ushort value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteUInt16(ushort value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteUInt32(uint value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteUInt32(uint value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteUInt64(ulong value)
    {
        throw new NotImplementedException();
    }

    public IByteBuffer WriteUInt64(ulong value, Endianless endianless)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.Uninitialized);
        }

        _readIdx = 0;
        _writtenIdx = 0;
    }

    public int SetBytes(int index, IReadOnlyByteBuffer src)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, IReadOnlyByteBuffer src, int length)
    {
        throw new NotImplementedException();
    }

    public int SetBytes(int index, IReadOnlyByteBuffer src, int srcIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int GetBytes(int index, IByteBuffer dest)
    {
        throw new NotImplementedException();
    }

    public int GetBytes(int index, IByteBuffer dest, int length)
    {
        throw new NotImplementedException();
    }

    public int GetBytes(int index, IByteBuffer dest, int destIndex, int length)
    {
        throw new NotImplementedException();
    }

    private void UpdateComponentsOffset(int componentIndex)
    {
        int count = _components.Count;
        for (int i = componentIndex + 1; i < count; ++i)
        {
            Component component = _components[i];
            component.AdjustOffset(_components[i - 1]);
        }
    }

    private Component? FindComponent(int offset)
    {
        int index = IndexOfComponent(offset);
        return index >= 0 ? _components[index] : null;
    }

    private int IndexOfComponent(int offset)
    {
        if (_components.Count <= 0)
        {
            return -1;
        }

        if (offset == 0)
        {
            _lastAccessed.Set(0, _components[0]);
            return 0;
        }

        Component tail = _components[^1];
        if (tail.Includes(offset))
        {
            int index = _components.Count - 1;
            _lastAccessed.Set(index, tail);
            return index;
        }

        Component? lastAccessed = _lastAccessed.Component;
        if (lastAccessed != null && lastAccessed.Includes(offset))
        {
            return _lastAccessed.Index;
        }

        int low = 0;
        int high = _components.Count;
        while (low <= high)
        {
            int mid = (low + high) >> 1;
            Component component = _components[mid];
            if (offset > component.EndOffset)
            {
                low = mid + 1;
            }
            else if (offset < component.BeginOffset)
            {
                high = mid - 1;
            }
            else
            {
                _lastAccessed.Set(mid, component);
                return mid;
            }
        }

        return -1;
    }

    private void ThrowIfComponentIndexOutOfRange(int componentIndex)
    {
        if (componentIndex < 0 || componentIndex > _components.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(componentIndex), componentIndex, null);
        }
    }

    private void ThrowIfInvalidByteBuffer(IByteBuffer byteBuffer)
    {
        if (byteBuffer == null)
        {
            throw new ArgumentNullException(nameof(byteBuffer));
        }

        if (!byteBuffer.IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.AlreadyReleased);
        }
    }

    private sealed class LastAccessedComponent
    {
        private int _index;

        private Component? _component;

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public Component? Component
        {
            get
            {
                return _component;
            }
        }

        public LastAccessedComponent()
        {
            Reset();
        }

        public void Set(int index, Component component)
        {
            _index = index;
            _component = component;
        }

        public void Reset()
        {
            _index = -1;
            _component = null;
        }
    }

    private sealed class Component
    {
        private readonly IByteBuffer _byteBuffer;

        private readonly int _length;

        private int _beginOffset;

        private int _endOffset;

        public IByteBuffer ByteBuffer
        {
            get
            {
                return _byteBuffer;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public int BeginOffset
        {
            get
            {
                return _beginOffset;
            }
            set
            {
                _beginOffset = value;
            }
        }

        public int EndOffset
        {
            get
            {
                return _endOffset;
            }
            set
            {
                _endOffset = value;
            }
        }

        public Component(IByteBuffer byteBuffer)
            : this(byteBuffer, 0)
        {
        }

        public Component(IByteBuffer byteBuffer, int beginOffset)
        {
            _byteBuffer = byteBuffer;
            _length = byteBuffer.ReadableBytes;
            AdjustOffset(beginOffset);
        }

        public void AdjustOffset(Component prevComponent)
        {
            AdjustOffset(prevComponent._endOffset);
        }

        public void AdjustOffset(int beginOffset)
        {
            _beginOffset = beginOffset;
            _endOffset = beginOffset + (_length - 1);
        }

        public bool Includes(int offset)
        {
            return offset >= _beginOffset && offset <= _endOffset;
        }

        public void Release()
        {
            _byteBuffer.Release();
        }
    }
}
