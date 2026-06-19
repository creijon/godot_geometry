using System.Collections.Generic;

public class Bitmask
{
    const int _wordPow2 = 5;
    const int _wordSize = 1 << _wordPow2;
    const int _wordMask = _wordSize - 1;

    List<uint> _data;
    int _startIndex;
    int _bitCount;
    int _wordCount;
    
    // Create a bitmask with an internal list.
    public Bitmask(int bitCount)
    {
        _startIndex = 0;
        _bitCount = bitCount;
        _wordCount = _bitCount >> _wordPow2;
        _data = new List<uint>(_wordCount);
        for (int i = 0; i < _wordCount; i++)
        {
            _data.Add(0);
        }
    }

    // Create a bitmask with an externally managed list.
    public Bitmask(List<uint> list, int index, int bitCount)
    {
        _data = list;
        _startIndex = index;
        _bitCount = bitCount;
        _wordCount = _bitCount >> _wordPow2;

        // Make sure that the list is large enough to hold the bit mask.
        while (_data.Count < _startIndex + _wordCount)
        {
            _data.Add(0);
        }
    }

    public bool Get(int bit)
    {
        int wordIndex = bit >> _wordPow2;
        int wordBit = bit - (wordIndex << _wordPow2);
        uint masked = _data[wordIndex + _startIndex] & (1u << wordBit);

        return (masked != 0);
    }

    public void Set(int bit)
    {
        int wordIndex = bit >> _wordPow2;
        int wordBit = bit - (wordIndex << _wordPow2);
        _data[wordIndex + _startIndex] |= (1u << wordBit);
    }

    public void Set(int bit, bool value)
    {
        int wordIndex = bit >> _wordPow2;
        int wordBit = bit - (wordIndex << _wordPow2);
        _data[wordIndex + _startIndex] &= ~(1u << wordBit);
        _data[wordIndex + _startIndex] |= value ? (1u << wordBit) : 0;
    }

    public void Clear(int bit)
    {
        int wordIndex = bit >> _wordPow2;
        int wordBit = bit - (wordIndex << _wordPow2);
        _data[wordIndex + _startIndex] &= ~(1u << wordBit);
    }

    public void Reset(bool state)
    {
        uint value = state ? ~0u : 0u;
        int wordCount = _bitCount >> _wordPow2;

        for (int i = _startIndex; i < _startIndex + wordCount; ++i)
        {
            _data[i] = value;
        }
    }

    public bool this[int key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    // Returns the number of bits set before `bit`. 
    public uint PopCountBefore(int bit)
    {
        int wordIndex = bit >> _wordPow2;
        uint count = 0;

        for (int i = _startIndex; i < _startIndex + wordIndex; ++i)
        {
            count += BitOps.PopCount(_data[i]);
        }

        // For anything other than the first bit in the word, count the bits in this one too.
        if (bit != wordIndex << _wordPow2)
        {
            int wordBit = bit - (wordIndex << _wordPow2);
            uint masked = _data[wordIndex + _startIndex] & ((1u << wordBit) - 1);
            count += BitOps.PopCount(masked);
        }

        return count;
    }

    public uint PopCountAfter(int bit)
    {
        int wordIndex = bit >> _wordPow2;
        uint count = 0;

        for (int i = wordIndex + 1; i < _wordCount; ++i)
        {
            count += BitOps.PopCount(_data[i + _startIndex]);
        }

        // For anything other than the last bit in the word, count the bits in this one too.
        if (bit != (wordIndex << _wordPow2) + _wordMask)
        {
            int wordBit = bit - (wordIndex << _wordPow2) + 1;
            uint masked = _data[wordIndex + _startIndex] & ~((1u << wordBit) - 1);
            count += (masked != 0) ? BitOps.PopCount(masked) : 0;
        }

        return count;
    }

    public bool AnySetAfter(int bit)
    {
        int wordIndex = bit >> _wordPow2;

        for (int i = wordIndex + 1; i < _wordCount; ++i)
        {
            if (_data[i + _startIndex] != 0)
            {
                return true;
            }
        }

        // For anything other than the last bit in the word, check the bits in this one too.
        if (bit != (wordIndex << _wordPow2) + _wordMask)
        {
            int wordBit = bit - (wordIndex << _wordPow2) + 1;
            uint masked = _data[wordIndex + _startIndex] & ~((1u << wordBit) - 1);
            if (masked != 0)
            {
                return true;
            }
        }

        return false;
    }

    public uint PopCount()
    {
        uint count = 0;
        for (int i = _startIndex; i < _startIndex + _wordCount; ++i)
        {
            count += BitOps.PopCount(_data[i]);
        }

        return count;
    }

    public void CopyTo(List<uint> words, int position)
    {
        for (int i = _startIndex; i < _startIndex + _wordCount; ++i)
        {
            words[position++] = _data[i];
        }
    }

    public int Length { get { return _bitCount; } }
    public int WordLength { get { return _wordCount; } }
}
