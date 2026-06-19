using System.Collections.Generic;

public class SparseArray<T>
{
    Bitmask _bitMask;
    List<T> _data;

    public SparseArray(int capacity)
    {
        _bitMask = new Bitmask(capacity);
        _data = new List<T>();
    }

    public SparseArray(T[] denseArray)
    {
        _bitMask = new Bitmask(denseArray.Length);
        _data = new List<T>();

        for (int i = 0; i < denseArray.Length; ++i)
        {
            if (denseArray[i] != null)
            {
                Add(i, denseArray[i]);
            }
        }
    }

    public SparseArray(List<T> denseList)
    {
        _bitMask = new Bitmask(denseList.Count);
        _data = new List<T>();

        for (int i = 0; i < denseList.Count; ++i)
        {
            if (denseList[i] != null)
            {
                Add(i, denseList[i]);
            }
        }
    }

    public void Add(int index, T value)
    {
        _data.Add(value);
        _bitMask.Set(index);
    }

    public void Insert(int index, T value)
    {
        uint packedIndex = _bitMask.PopCountBefore(index);

        if (_bitMask[index])
        {
            // If the element is already populated, overwrite.
            _data[(int)packedIndex] = value;
            return;
        }

        _data.Insert((int)packedIndex, value);
        _bitMask.Set(index);
    }

    public void RemoveAt(int index)
    {
        if (!_bitMask[index])
        {
            // If the element is not populated, ignore.
            return;
        }

        uint packedIndex = _bitMask.PopCountBefore(index);
        _data.RemoveAt((int)packedIndex);
        _bitMask.Clear(index);
    }

    // Quickly test whether an element is populated.
    public bool Exists(int index)
    {
        return _bitMask[index];
    }

    public T Get(int index)
    {
        if (!_bitMask[index])
        {
            // If the element is not populated, return null or default.
            return default(T);
        }

        uint packedIndex = _bitMask.PopCountBefore(index);

        return _data[(int)packedIndex];
    }

    public T this[int key]
    {
        get => Get(key);
        set => Insert(key, value);
    }

    public int Count { get { return _data.Count; } }

    public int Capacity { get { return _bitMask.Length; } }

    public Bitmask Bitmask { get { return _bitMask; } }
}
