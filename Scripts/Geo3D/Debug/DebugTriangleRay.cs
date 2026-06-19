using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugTriangleRay : Node3D
    {
        [Export]
        public DrawTriangle _tri;

        [Export]
        public DrawRay _ray;

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_tri == null || _ray == null) return;

                float t = 0.0f;

                if (Intersect.Test(_ray._ray, _tri._tri, out t))
                {
                    _ray._color = new Color(0.0f, 1.0f, 0.0f);;
                }
                else
                {
                    _ray._color = new Color(1.0f, 0.0f, 0.0f);;
                }

                var p = _ray._ray.Position(t);
                var plane = _tri._tri.Plane();
                GeoDebug.Wireframe.Line(p, p + plane.n, _ray._color);
            }
        }
    }

}