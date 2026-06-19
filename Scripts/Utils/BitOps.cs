class BitOps
{
    // Replacement for System.Numerics.BitOperations.PopCount
    public static uint PopCount(ulong x)
    {
        x -= (x >> 1) & 0x5555555555555555ul;
        x = (x & 0x3333333333333333ul) + (x >> 2 & 0x3333333333333333ul);
        x = (x + (x >> 4)) & 0xf0f0f0f0f0f0f0ful;
        return (uint)((x * 0x0101010101010101ul) >> 56);
    }

    public static uint PopCount(uint x)
    {
        x -= (x >> 1) & 0x55555555u;
        x = (x & 0x33333333u) + ((x >> 2) & 0x33333333u);
        x = (x + (x >> 4)) & 0x0F0F0F0Fu;
        return (x * 0x01010101u) >> 24;
    }

    public static uint PopCount(byte x)
    {
        uint v = x;

        v -= ((v >> 1) & 0x55);
        v = (v & 0x33) + ((v >> 2) & 0x33);
        v = (v + (v >> 4)) & 0x0F;

        return v;
    }

    public static uint PopCountBefore(ulong x, int bit)
    {
        ulong masked = x & ((1UL << bit) - 1);
        return PopCount(x & masked);
    }

    public static uint PopCountBefore(uint lw, uint hw, int bit)
    {
        uint maskedL = lw & ((bit < 32) ? (0x7FFFFFFFu >> (31 - bit)) : 0xFFFFFFFFu);
        uint maskedH = hw & ((bit > 31) ? (0x7FFFFFFFu >> (63 - bit)) : 0);

        return PopCount(maskedL) + PopCount(maskedH);
    }

    public static int RoundUpPow2(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;

        return v;
    }

    public static int Mask(int bits)
    {
        return -1 ^ (-1 << bits);
    }
}
