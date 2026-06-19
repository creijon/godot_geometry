using Godot;

namespace Geo2D
{
    // A line segment connecting two points on the XY plane.
    public class Edge
    {
        public Edge(Vector2 v0, Vector2 v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }

        public Vector2 Axis => v1 - v0;
        public Vector2 Centre => v0 + Axis * 0.5f;

        public Vector2 Direction()
        {
            return Axis.Normalized();
        }

        public Vector2 v0;
        public Vector2 v1;
    }
}
