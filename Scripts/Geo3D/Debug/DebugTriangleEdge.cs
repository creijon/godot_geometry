using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugTriangleEdge : Node3D
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
                if (_tri._tri == null) return;
                if (_edge._edge == null) return;

                float t = 0.0f;

                if (Intersect.Test(_edge._edge, _tri._tri, out t))
                {
                    _edge._color = new Color(0.0f, 1.0f, 0.0f);
                    var plane = _tri._tri.Plane();
                    var p = _edge._edge.v0.Lerp(_edge._edge.v1, t);
                    DebugDraw.DrawLine(p, p + plane.n * 3.0f, _edge._color);
                }
                else
                {
                    _edge._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }
}