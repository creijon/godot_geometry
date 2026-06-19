using Godot;

// A triad of integers.
public class Int3
{
    public Int3(int val)
    {
        X = val;
        Y = val;
        Z = val;
    }

    public Int3(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Int3(Int3 rhs)
    {
        X = rhs.X;
        Y = rhs.Y;
        Z = rhs.Z;
    }

    public Int3(Vector3 vec)
    {
        X = Mathf.FloorToInt(vec.X);
        Y = Mathf.FloorToInt(vec.Y);
        Z = Mathf.FloorToInt(vec.Z);
    }

    public void Min(Int3 min)
    {
        X = Mathf.Min(X, min.X);
        Y = Mathf.Min(Y, min.Y);
        Z = Mathf.Min(Z, min.Z);
    }

    public void Max(Int3 max)
    {
        X = Mathf.Max(X, max.X);
        Y = Mathf.Max(Y, max.Y);
        Z = Mathf.Max(Z, max.Z);
    }

    static public Int3 Zero => new Int3(0, 0, 0);
    
    static public Int3 Min(Int3 a, Int3 b)
    {
        Int3 min = new Int3(a);
        min.Min(b);
        return min;
    }

    static public Int3 Max(Int3 a, Int3 b)
    {
        Int3 max = new Int3(a);
        max.Max(b);
        return max;
    }

    public static Int3 operator +(Int3 a, Int3 b) => new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Int3 operator *(Int3 a, Int3 b) => new Int3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

    public static Int3 operator *(Int3 a, int b) => new Int3(a.X * b, a.Y * b, a.Z * b);

    public int Total
    {
        get { return X * Y * Z; }
    }

    public static explicit operator Vector3(Int3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public int X, Y, Z;
}
