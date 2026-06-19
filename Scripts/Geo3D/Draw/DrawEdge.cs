using Godot;

namespace Geo3D
{
    [Tool]
    public partial class DrawEdge : Node3D
    {
        [Export] 
        public Node3D _v0;
        [Export] 
        public Node3D _v1;
        [Export] 
        public Color _color;

        public Edge _edge;

        void Reset()
        {
            _edge = null;
            if (_v0 != null && _v1 != null)
            {
                _edge = new Edge(_v0.GlobalPosition, _v1.GlobalPosition);
            }
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

                if (_edge != null)
                {
                    GeoDebug.Wireframe.Line(_edge.v0, _edge.v1, _color);
                }
            }
        }
    }
}
