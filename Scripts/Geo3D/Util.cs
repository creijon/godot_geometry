using Godot;

namespace Geo3D
{
    class Util
    {
        public static Vector2 XY(Vector3 v) => new Vector2(v.X, v.Y);

        public static Vector2 YZ(Vector3 v) => new Vector2(v.Y, v.Z);

        public static Vector2 ZX(Vector3 v) => new Vector2(v.Z, v.X);

        public static Vector3 Reciprocal(Vector3 v) => new Vector3(1.0f / v.X, 1.0f / v.Y, 1.0f / v.Z);

        public static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.X), Mathf.Abs(v.Y), Mathf.Abs(v.Z));

        public static float MinCoefficient(Vector3 v)
        {
            return (v.X < v.Z) ? ((v.X < v.Y) ? v.X : v.Y) : ((v.Y < v.Z) ? v.Y : v.Z);
        }

        public static float MaxCoefficient(Vector3 v)
        {
            return (v.X > v.Z) ? ((v.X > v.Y) ? v.X : v.Y) : ((v.Y > v.Z) ? v.Y : v.Z);
        }
    }
}
