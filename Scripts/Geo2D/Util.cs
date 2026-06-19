using Godot;

namespace Geo2D
{
    class Util
    {
        public static Vector3 XY(Vector2 v) => new Vector3(v.X, v.Y, 0.0f);

        public static Vector3 YZ(Vector2 v) => new Vector3(0.0f, v.X, v.Y);

        public static Vector3 ZX(Vector2 v) => new Vector3(v.Y, 0.0f, v.X);

        public static Vector2 Abs(Vector2 v) => new Vector2(Mathf.Abs(v.X), Mathf.Abs(v.Y));

        public static float MaxCoefficient(Vector2 v)
        {
            return (v.X > v.Y) ? v.X : v.Y;
        }

        public static float SignedTriArea(Vector2 a, Vector2 b, Vector2 c)
        {
            var ca = a - c;
            var cb = b - c;
            return ca.X * cb.Y - ca.Y * cb.X;
        }
    }
}
