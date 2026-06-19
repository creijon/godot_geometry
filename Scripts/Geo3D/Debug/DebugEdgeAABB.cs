using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugEdgeAABB : Node3D
    {
        [Export] 
        public DrawEdge _edge;

        [Export] 
        public DrawAABB _aabb;

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_aabb == null || _edge == null) return;
                if (_aabb._aabb == null) return;
                if (_edge._edge == null) return;

                if (Intersect.Test(_edge._edge, _aabb._aabb))
                {
                    _edge._color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    _edge._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }

}