using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DrawRay : Node3D
    {
        [Export]
        public Color _color;
        [Export]
        public float _length = 3.0f;
        public Ray _ray;

        void Reset()
        {
            _ray = new Ray(GlobalPosition, Transform.Basis.Z.Normalized());
        }

        public override void _Ready()
        {
            Reset();
        }

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                Reset();

                var tip = _length * 0.1f;
                var end = _ray.origin + _ray.dir * _length;
                var head = _ray.origin + _ray.dir * (_length - tip);
                var offset = Transform.Basis.X * tip * 0.5f;
                DebugDraw.DrawLine(_ray.origin, head, _color);
                DebugDraw.DrawLine(head - offset, head + offset, _color);
                DebugDraw.DrawLine(head - offset, end, _color);
                DebugDraw.DrawLine(head + offset, end, _color);
            }
        }
    }
}