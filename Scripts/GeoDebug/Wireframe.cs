using Godot;
using System.Collections.Generic;

namespace GeoDebug
{
[Tool]
public partial class Wireframe : Node2D
{
    // Draw a line between two world-space points with the current color
    public static void Line(Vector3 p0, Vector3 p1)
    {
        _instance.segments.Add(new Segment { p0 = p0, p1 = p1, color = _instance.currentColor });
    }

    // Draw a line between two world-space points in a defined color
    public static void Line(Vector3 p0, Vector3 p1, Color color)
    {
        _instance.currentColor = color;
        Line(p0, p1);
    }

    public static void Box(Vector3 min, Vector3 max, Color color, Transform3D transform)
    {
        Vector3 v000 = transform * min;
        Vector3 v001 = transform * new Vector3(max.X, min.Y, min.Z);
        Vector3 v010 = transform * new Vector3(min.X, max.Y, min.Z);
        Vector3 v011 = transform * new Vector3(max.X, max.Y, min.Z);
        Vector3 v100 = transform * new Vector3(min.X, min.Y, max.Z);
        Vector3 v101 = transform * new Vector3(max.X, min.Y, max.Z);
        Vector3 v110 = transform * new Vector3(min.X, max.Y, max.Z);
        Vector3 v111 = transform * max;

        _instance.currentColor = color;
        Line(v000, v001);
        Line(v001, v011);
        Line(v011, v010);
        Line(v010, v000);

        Line(v100, v101);
        Line(v101, v111);
        Line(v111, v110);
        Line(v110, v100);

        Line(v000, v100);
        Line(v001, v101);
        Line(v010, v110);
        Line(v011, v111);
    }

    public override void _Process(double delta)
    {
        if (segments.Count > 0)
		{
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        Camera3D camera;
        Vector2 viewport_size;

        if (Engine.IsEditorHint())
        {
            var viewport = EditorInterface.Singleton.GetEditorViewport3D();
            viewport_size = new Vector2(viewport.Size.X, viewport.Size.Y);
            camera = viewport.GetCamera3D();
            // In the editor we need to account for the viewport offset when drawing lines.
            DrawSetTransform(viewport.GetScreenTransform().Origin);
        }
        else
        {
            camera = GetViewport().GetCamera3D();
            viewport_size = GetViewport().GetVisibleRect().Size;
        }

        var near_plane_n = camera.GlobalBasis.Z;
        var near_plane_d = near_plane_n.Dot(camera.GlobalPosition) - camera.Near;

        foreach(var segment in segments)
        {
            var p0 = segment.p0;
            var p1 = segment.p1;

            // Clip the line against the camera's near plane.
            if (!LineClipper.ClipAgainstPlane(ref p0, ref p1, near_plane_n, near_plane_d))
            {
                // Line is completely behind the near plane, skip drawing.
                continue;
            }

            // Project the points to screen space.
            Vector2 s0 = camera.UnprojectPosition(p0);
            Vector2 s1 = camera.UnprojectPosition(p1);

            // Clip the lines against the viewport bounds.
            if (!LineClipper.ClipAgainstRectangle(ref s0, ref s1, Vector2.Zero, viewport_size))
            {
                // Line is completely outside the viewport, skip drawing.
                continue;
            }
            
            DrawLine(s0, s1, segment.color, -1, true);
        }

        segments.Clear();
    }

    static Wireframe _instance;

    Wireframe()
    {
        _instance = this;
    }

    struct Segment
    {
        public Vector3 p0;
        public Vector3 p1;
        public Color color;
    }

    List<Segment> segments = new List<Segment>();
    Color currentColor = Colors.White;
}

}