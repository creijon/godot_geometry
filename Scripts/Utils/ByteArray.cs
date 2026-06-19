using System.Collections.Generic;

// Storage for uint8, uint24 and uint32 values in the same array.
public class ByteArray
{
    public ByteArray(int length)
    {
        _bytes = new List<byte>(length);
    }

    // uint8 insert, get and set
    public void AddU8(byte value)
    {
        _bytes.Add(value);
    }

    public void PadU8(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            _bytes.Add(0);
        }
    }

    public byte GetU8(int index)
    {
        return _bytes[index];
    }

    public void SetU8(int index, byte value)
    {
        _bytes[index] = value;
    }

    public void AddU8(byte[] values)
    {
        foreach (byte value in values)
        {
            AddU8(value);
        }
    }

    // uint24 insert, get and set
    public void AddU24(uint value)
    {
        _bytes.Add((byte)(value & 0xFF));
        _bytes.Add((byte)((value >> 8) & 0xFF));
        _bytes.Add((byte)((value >> 16) & 0xFF));
    }

    public void PadU24(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
        }
    }

    public uint GetU24(int index)
    {
        uint value = (uint)_bytes[index++]
                   | (uint)_bytes[index++] << 8
                   | (uint)_bytes[index] << 16;

        return value;
    }

    public void SetU24(int index, uint value)
    {
        _bytes[index++] = (byte)(value & 0xFF);
        _bytes[index++] = (byte)((value >> 8) & 0xFF);
        _bytes[index]   = (byte)((value >> 16) & 0xFF);
    }

    public void AddU24(uint[] values)
    {
        foreach (uint value in values)
        {
            AddU24(value);
        }
    }

    // uint32 insert, get and set
    public void AddU32(uint value)
    {
        _bytes.Add((byte)(value & 0xFF));
        _bytes.Add((byte)((value >> 8) & 0xFF));
        _bytes.Add((byte)((value >> 16) & 0xFF));
        _bytes.Add((byte)(value >> 24));
    }

    public void PadU32(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
        }
    }

    public uint GetU32(int index)
    {
        uint value = (uint)_bytes[index++]
                   | (uint)_bytes[index++] << 8
                   | (uint)_bytes[index++] << 16
                   | (uint)_bytes[index] << 24;

        return value;
    }

    public void SetU32(int index, uint value)
    {
        _bytes[index++] = (byte)(value & 0xFF);
        _bytes[index++] = (byte)((value >> 8) & 0xFF);
        _bytes[index++] = (byte)((value >> 16) & 0xFF);
        _bytes[index]   = (byte)(value >> 24);
    }

    public void AddU32(uint[] values)
    {
        foreach (uint value in values)
        {
            AddU32(value);
        }
    }


    // uint64 insert, get and set
    public void AddU64(ulong value)
    {
        _bytes.Add((byte)(value & 0xFF));
        _bytes.Add((byte)((value >> 8) & 0xFF));
        _bytes.Add((byte)((value >> 16) & 0xFF));
        _bytes.Add((byte)((value >> 24) & 0xFF));
        _bytes.Add((byte)((value >> 32) & 0xFF));
        _bytes.Add((byte)((value >> 40) & 0xFF));
        _bytes.Add((byte)((value >> 48) & 0xFF));
        _bytes.Add((byte)(value >> 56));
    }

    public void PadU64(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
            _bytes.Add(0);
        }
    }

    public ulong GetU64(int index)
    {
        ulong value = (uint)_bytes[index++]
                    | (uint)_bytes[index++] << 8
                    | (uint)_bytes[index++] << 16
                    | (uint)_bytes[index++] << 24
                    | (uint)_bytes[index++] << 32
                    | (uint)_bytes[index++] << 40
                    | (uint)_bytes[index++] << 48
                    | (uint)_bytes[index++] << 56;

        return value;
    }

    public void SetU64(int index, ulong value)
    {
        _bytes[index++] = (byte)(value & 0xFF);
        _bytes[index++] = (byte)((value >> 8) & 0xFF);
        _bytes[index++] = (byte)((value >> 16) & 0xFF);
        _bytes[index++] = (byte)((value >> 24) & 0xFF);
        _bytes[index++] = (byte)((value >> 32) & 0xFF);
        _bytes[index++] = (byte)((value >> 40) & 0xFF);
        _bytes[index++] = (byte)((value >> 48) & 0xFF);
        _bytes[index]   = (byte)(value >> 56);
    }

    public void AddU64(ulong[] values)
    {
        foreach (ulong value in values)
        {
            AddU64(value);
        }
    }

    public int Count { get { return _bytes.Count; } }

    List<byte> _bytes;
}
