using Godot;
using System.Collections.Generic;

[Tool]
public partial class DebugDraw : Node2D
{
    // Draw a line between two world-space points with the current color
    public static void DrawLine(Vector3 p0, Vector3 p1)
    {
        _instance.lines.Add(new Line { p0 = p0, p1 = p1, color = _instance.currentColor });
    }

    // Draw a line between two world-space points in a defined color
    public static void DrawLine(Vector3 p0, Vector3 p1, Color color)
    {
        _instance.currentColor = color;
        DrawLine(p0, p1);
    }

    public override void _Process(double delta)
    {
        if (lines.Count > 0)
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

        for (int i = 0; i < lines.Count; i++)
        {
            var p0 = lines[i].p0;
            var p1 = lines[i].p1;

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
            
            DrawLine(s0, s1, lines[i].color, -1, true);
        }

        lines.Clear();
    }

    static DebugDraw _instance;

    DebugDraw()
    {
        _instance = this;
    }

    struct Line
    {
        public Vector3 p0;
        public Vector3 p1;
        public Color color;
    }

    List<Line> lines = new List<Line>();
    Color currentColor = Colors.White;

}
