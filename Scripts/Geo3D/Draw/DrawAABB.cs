using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DrawAABB : Node3D
    {
        public Color _color;
        public AABB _aabb;

        [Export] 
        public Color color
        {
            get => _color;
            set
            {
                _color = value;
            }
        }

        void Reset()
        {
            _aabb = new AABB(GlobalTransform.Origin, GlobalTransform.Basis.Scale);
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

                Draw(_aabb.Min, _aabb.Max, _color, Transform3D.Identity);
            }
        }

        public static void Draw(Vector3 min, Vector3 max, Color color, Transform3D transform)
        {
            Vector3 v001 = new Vector3(max.X, min.Y, min.Z);
            Vector3 v010 = new Vector3(min.X, max.Y, min.Z);
            Vector3 v011 = new Vector3(max.X, max.Y, min.Z);
            Vector3 v100 = new Vector3(min.X, min.Y, max.Z);
            Vector3 v101 = new Vector3(max.X, min.Y, max.Z);
            Vector3 v110 = new Vector3(min.X, max.Y, max.Z);

            min = transform * min;
            v001 = transform * v001;
            v010 = transform * v010;
            v011 = transform * v011;
            v100 = transform * v100;
            v101 = transform * v101;
            v110 = transform * v110;
            max = transform * max;

            DebugDraw.DrawLine(min,  v001, color);
            DebugDraw.DrawLine(v001, v011, color);
            DebugDraw.DrawLine(v011, v010, color);
            DebugDraw.DrawLine(v010, min,  color);

            DebugDraw.DrawLine(v100, v101, color);
            DebugDraw.DrawLine(v101, max,  color);
            DebugDraw.DrawLine(max,  v110, color);
            DebugDraw.DrawLine(v110, v100, color);

            DebugDraw.DrawLine(min,  v100, color);
            DebugDraw.DrawLine(v001, v101, color);
            DebugDraw.DrawLine(v010, v110, color);
            DebugDraw.DrawLine(v011, max,  color);
        }
    }
}