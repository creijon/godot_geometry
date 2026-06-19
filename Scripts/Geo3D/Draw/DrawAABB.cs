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

                GeoDebug.Wireframe.Box(_aabb.Min, _aabb.Max, _color, Transform3D.Identity);
            }
        }
    }
}
