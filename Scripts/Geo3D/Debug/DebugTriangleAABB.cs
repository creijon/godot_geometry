using Godot;
namespace Geo3D
{
    [Tool]
    public partial class DebugTriangleAABB : Node3D
    {
        [Export]
        public DrawTriangle _tri;

        [Export]
        public DrawAABB _aabb;

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_aabb == null || _tri == null) return;

                if (Intersect.Test(_tri._tri, _aabb._aabb))
                {
                    _aabb._color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    _aabb._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }
}