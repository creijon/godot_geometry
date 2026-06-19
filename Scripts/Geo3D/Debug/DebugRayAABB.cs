using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DebugRayAABB : Node3D
    {
        [Export]
        public DrawRay _ray;

        [Export]
        public DrawAABB _aabb;


        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                if (_aabb == null || _ray == null) return;

                float t = 0.0f;

                if (Intersect.Test(_ray._ray, _aabb._aabb, out t))
                {
                    _ray._color = new Color(0.0f, 1.0f, 0.0f);
                    Vector3 hitPos = _ray._ray.Position(t);
                    GeoDebug.Wireframe.Box(-Vector3.One * 0.1f + hitPos, Vector3.One * 0.1f + hitPos, _ray._color, Transform3D.Identity);
                }
                else
                {
                    _ray._color = new Color(1.0f, 0.0f, 0.0f);
                }
            }
        }
    }
}
