using Godot;

namespace Geo3D
{
    public class Ray
    {
        public Ray(Vector3 origin, Vector3 dir)
        {
            this.origin = origin;
            this.dir = dir;
        }

        public Vector3 Position(float t)
        {
            return origin + dir * t;
        }

        public Vector3 origin;
        public Vector3 dir;
    }
}
