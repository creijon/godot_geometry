using Godot;

namespace Geo3D
{
    public class Edge
    {
        public Edge(Vector3 v0, Vector3 v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }

        public Vector3 Axis => v1 - v0;
        public Vector3 Centre => v0 + Axis * 0.5f;

        public Vector3 Direction()
        {
            return Axis.Normalized();
        }

        public Vector3 v0;
        public Vector3 v1;
    }
}
