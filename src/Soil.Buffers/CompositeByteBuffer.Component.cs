using System;

namespace Soil.Buffers;

public partial class CompositeByteBuffer
{
    private static int ComputeOffset(int index, int bytes, Component component)
    {
        return component.ByteBuffer.ReadIndex + (index + bytes - component.BeginOffset);
    }

    private static int ComputeDestOffset(int destIndex, int bytes)
    {
        return destIndex + bytes;
    }

    private static int MinLengthToGet(int length, int bytes, Component component)
    {
        return Math.Min(length - bytes, component.Length);
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
}
