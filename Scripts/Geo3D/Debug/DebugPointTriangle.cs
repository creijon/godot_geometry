using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugPointTriangle : Node3D
    {
        [Export]
        public Node3D _point;

        [Export]
        public DrawTriangle _tri;

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_tri == null || _point == null) return;

                var plane = _tri._tri.Plane();

                DebugDraw.DrawLine(_point.GlobalPosition, plane.Project(_point.GlobalPosition), new Color(1.0f, 1.0f, 0.0f));

                if (Intersect.Test(_point.GlobalPosition, _tri._tri))
                {
                    _tri._color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    _tri._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }

}