using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugAABBAABB : Node3D
    {
        [Export]
        public DrawAABB _aabb1;
    
        [Export]
        public DrawAABB _aabb2;


        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_aabb1 == null || _aabb2 == null) return;

                if (Intersect.Test(_aabb1._aabb, _aabb2._aabb))
                {
                    _aabb2._color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    _aabb2._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }
}