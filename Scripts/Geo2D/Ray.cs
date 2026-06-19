using Godot;

namespace Geo2D
{
    // A ray on the XY plane.
    // Defined by an origin and a direction vector of unit length.
    public class Ray
    {
        public Ray(Vector2 origin, Vector2 dir)
        {
            this.origin = origin;
            this.dir = dir;
        }

        public Vector2 Position(float t)
        {
            return origin + dir * t;
        }

        public Vector2 origin;
        public Vector2 dir;
    }
}
