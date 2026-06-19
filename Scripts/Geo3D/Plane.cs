using Godot;

namespace Geo3D
{
    public class Plane
    {
        public Plane(Vector3 n, float d)
        {
            this.n = n;
            this.d = d;
        }

        public float SignedDistance(Vector3 p)
        {
            return n.Dot(p) - d;
        }

        public Vector3 Project(Vector3 p)
        {
            return p - (SignedDistance(p) * n);
        }

        public Vector3 n;
        public float d;
    }
}
