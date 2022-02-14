
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Soil.Collections.Generics;

public class CopyOnWriteList<T> : IList<T>, IReadOnlyList<T>
{
    private volatile T[] _array;

    private readonly object _lock = new();

    public CopyOnWriteList()
    {
        _array = Array.Empty<T>();
    }

    public CopyOnWriteList(IEnumerable<T> enumerable)
    {
        T[] otherArr = enumerable is CopyOnWriteList<T> other
            ? other.GetArray()
            : enumerable.ToArray();

        int otherArrLen = otherArr.Length;
        if (otherArrLen <= 0)
        {
            _array = Array.Empty<T>();
            return;
        }

        T[] newArr = new T[otherArrLen];
        Array.Copy(otherArr, newArr, otherArrLen);

        _array = newArr;
    }

    public CopyOnWriteList(T[] array)
    {
        int arrLen = array.Length;
        if (arrLen <= 0)
        {
            _array = Array.Empty<T>();
            return;
        }

        T[] newArr = new T[arrLen];
        Array.Copy(array, newArr, arrLen);

        _array = newArr;
    }

    public T this[int index]
    {
        get
        {
            return GetArray()[index];
        }

        set
        {
            Insert(index, value);
        }
    }

    public int Count
    {
        get
        {
            return GetArray().Length;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public void Add(T item)
    {
        Monitor.Enter(_lock);
        try
        {
            T[] oldElements = GetArray();
            int oldLen = oldElements.Length;
            T[] newElements = new T[oldLen + 1];
            oldElements.CopyTo(newElements, 0);
            newElements[oldLen] = item;
            SetArray(newElements);
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    public void Clear()
    {
        Monitor.Enter(_lock);
        try
        {
            SetArray(Array.Empty<T>());
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    public bool Contains(T item)
    {
        T[] elements = GetArray();
        return elements.Contains(item);
    }

    public void CopyTo(T[] array, int index)
    {
        T[] elements = GetArray();
        elements.CopyTo(array, index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(GetArray());
    }

    public int IndexOf(T item)
    {
        return Array.IndexOf(GetArray(), item);
    }

    public void Insert(int index, T item)
    {
        Monitor.Enter(_lock);
        try
        {
            T[] oldElements = GetArray();
            int oldLen = oldElements.Length;
            if (index > oldLen || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            T[] newElements = new T[oldLen + 1];
            int moveCount = oldLen - index;
            if (moveCount > 0)
            {
                Array.Copy(oldElements, 0, newElements, 0, index);
                Array.Copy(oldElements, index, newElements, index + 1, moveCount);
            }

            newElements[index] = item;
            SetArray(newElements);
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    public bool Remove(T item)
    {
        Monitor.Enter(_lock);
        try
        {
            T[] oldElements = GetArray();
            int index = Array.IndexOf(oldElements, item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    public void RemoveAt(int index)
    {
        Monitor.Enter(_lock);
        try
        {
            T[] oldElements = GetArray();
            int oldLen = oldElements.Length;
            if (index >= oldLen || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            T[] newElements = new T[oldLen - 1];
            int moveCount = oldLen - index - 1;

            if (moveCount == 0)
            {
                Array.Copy(oldElements, 0, newElements, 0, oldLen - 1);
                SetArray(newElements);
                return;
            }

            if (moveCount < oldLen - 1)
            {
                Array.Copy(oldElements, 0, newElements, 0, index);
            }
            Array.Copy(oldElements, index + 1, newElements, index, moveCount);
            SetArray(newElements);
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(GetArray());
    }

    private struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly T[] _snapshot;

        private int _idx;

        public Enumerator(T[] snapshot)
        {
            _snapshot = snapshot;
            _idx = -1;
        }

        public T Current
        {
            get
            {
                try
                {
                    return _snapshot[_idx];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current!;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _idx++;
            return _idx < _snapshot.Length;
        }

        public void Reset()
        {
            _idx = 0;
        }
    }

    private T[] GetArray()
    {
        return _array;
    }

    private void SetArray(T[] arr)
    {
        _array = arr;
    }
}
