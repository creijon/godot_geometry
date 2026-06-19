using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DrawTriangle : Node3D
    {
        [Export] 
        public Node3D[] _verts = new Node3D[3];
        [Export] 
        public Color _color;
        public Triangle _tri;

        void Reset()
        {
            if (_verts[0] != null && _verts[1] != null && _verts[2] != null)
            {
                _tri = new Triangle(_verts[0].GlobalPosition,
                                    _verts[1].GlobalPosition,
                                    _verts[2].GlobalPosition);
            }
        }

        // Start is called before the first frame update
        public override void _Ready()
        {
            Reset();
        }

        public override void _Process(double delta)
        {
            if (IsVisibleInTree())
            {
                Reset();

                DebugDraw.DrawLine(_tri.v0, _tri.v1, _color);
                DebugDraw.DrawLine(_tri.v1, _tri.v2, _color);
                DebugDraw.DrawLine(_tri.v2, _tri.v0, _color);
            }
        }
    }
}