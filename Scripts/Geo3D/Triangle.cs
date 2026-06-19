using Godot;

namespace Geo3D
{
    public class Triangle
    {
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
        }

        public Edge Edge0 => new Edge(v0, v1);
        public Edge Edge1 => new Edge(v1, v2);
        public Edge Edge2 => new Edge(v2, v0); 
        public Geo2D.Triangle XY => new Geo2D.Triangle(Util.XY(v0), Util.XY(v1), Util.XY(v2));
        public Geo2D.Triangle YZ => new Geo2D.Triangle(Util.YZ(v0), Util.YZ(v1), Util.YZ(v2));
        public Geo2D.Triangle ZX => new Geo2D.Triangle(Util.ZX(v0), Util.ZX(v1), Util.ZX(v2));
        public Vector3 Cross => (v1 - v0).Cross(v1 - v2);

        public AABB Bounds()
        {
            var min = v0.Min(v1).Min(v2);
            var max = v0.Max(v1).Max(v2);

            return new AABB(min, max, true);
        }

        public Vector3 Normal()
        {
            return Cross.Normalized();
        }

        public Plane Plane()
        {
            Vector3 n = Normal();
            float d = v0.Dot(n);

            return new Plane(n, d);
        }

        public Vector2 Barycentric(Vector3 p)
        {
            Vector3 v1 = this.v1 - this.v0;
            Vector3 v0 = this.v2 - this.v0;
            Vector3 v2 = p - this.v0;

            float dot00 = v0.Dot(v0);
            float dot01 = v0.Dot(v1);
            float dot02 = v0.Dot(v2);
            float dot11 = v1.Dot(v1);
            float dot12 = v1.Dot(v2);

            float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return new Vector2(u, v);
        }

        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;
    }
}
