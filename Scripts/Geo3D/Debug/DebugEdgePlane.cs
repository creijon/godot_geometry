using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugEdgePlane : Node3D
    {
        [Export]
        public DrawTriangle _tri;

        [Export]
        public DrawEdge _edge;

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_tri == null || _edge == null) return;

                var plane = _tri._tri.Plane();
                float t = 0.0f;

                if (Intersect.Test(_edge._edge, plane, out t))
                {
                    _edge._color = new Color(0.0f, 1.0f, 0.0f);
                    Vector3 intersection = _edge._edge.v0.Lerp(_edge._edge.v1, t);

                    GeoDebug.Wireframe.Line(intersection, intersection + plane.n, _edge._color);
                }
                else
                {
                    _edge._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }

}