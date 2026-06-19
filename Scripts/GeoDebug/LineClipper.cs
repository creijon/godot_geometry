using Godot;

namespace GeoDebug
{
public class LineClipper
{
    private const int INSIDE = 0; // 0000
    private const int LEFT = 1;   // 0001
    private const int RIGHT = 2;  // 0010
    private const int BOTTOM = 4; // 0100
    private const int TOP = 8;    // 1000

    private static int ComputeOutCode(Vector2 p, Vector2 min, Vector2 max)
    {
        int code = INSIDE;

        code |= (p.X < min.X) ? LEFT : 0;
        code |= (p.X > max.X) ? RIGHT : 0;
        code |= (p.Y < min.Y) ? BOTTOM : 0;
        code |= (p.Y > max.Y) ? TOP : 0;

        return code;
    }

    /// Clips a line segment (p0, p1) against a rectangle (rectMin, rectMax)
    /// Based on the Cohen-Sutherland algorithm.
    public static bool ClipAgainstRectangle(ref Vector2 p0, ref Vector2 p1, Vector2 rectMin, Vector2 rectMax)
    {
        int outcode0 = ComputeOutCode(p0, rectMin, rectMax);
        int outcode1 = ComputeOutCode(p1, rectMin, rectMax);

        while (true)
        {
            if ((outcode0 | outcode1) == 0)
            {
                // Both points are within the rectangle.
                return true;
            }
            if ((outcode0 & outcode1) != 0)
            {
                // Both points are in the same outside region, so the line is not visible.
                return false;
            }

            // Line needs clipping
            float x = 0, y = 0;
            int outcode = outcode0 != 0 ? outcode0 : outcode1;

            // Intersection math shifting to the arbitrary rectangle boundaries
            if ((outcode & TOP) != 0)
            {
                x = p0.X + (p1.X - p0.X) * (rectMax.Y - p0.Y) / (p1.Y - p0.Y);
                y = rectMax.Y;
            }
            else if ((outcode & BOTTOM) != 0)
            {
                x = p0.X + (p1.X - p0.X) * (rectMin.Y - p0.Y) / (p1.Y - p0.Y);
                y = rectMin.Y;
            }
            else if ((outcode & RIGHT) != 0)
            {
                y = p0.Y + (p1.Y - p0.Y) * (rectMax.X - p0.X) / (p1.X - p0.X);
                x = rectMax.X;
            }
            else if ((outcode & LEFT) != 0)
            {
                y = p0.Y + (p1.Y - p0.Y) * (rectMin.X - p0.X) / (p1.X - p0.X);
                x = rectMin.X;
            }

            // Push updates back to the clipped endpoint and refresh its code
            if (outcode == outcode0)
            {
                p0.X = x;
                p0.Y = y;
                outcode0 = ComputeOutCode(p0, rectMin, rectMax);
            }
            else
            {
                p1.X = x;
                p1.Y = y;
                outcode1 = ComputeOutCode(p1, rectMin, rectMax);
            }
        }
    }

    /// Clips a line segment (p0, p1) against a plane (planeN, planeD).
    public static bool ClipAgainstPlane(ref Vector3 p0, ref Vector3 p1, Vector3 planeN, float planeD)
    {
        var d0 = planeN.Dot(p0) - planeD;
        var d1 = planeN.Dot(p1) - planeD;

        if (d0 <= 0.0f && d1 <= 0.0f)
        {
            // Both points are in front of the plane, no need to clip.
            return true;
        }

        if (d0 > 0.0f && d1 > 0.0f)
        {
            // Both points are behind the plane, reject.
            return false;
        }

        // Calculate intesection of line segment with plane.
        var t = d0 / (d0 - d1);
        var p = p0 + t * (p1 - p0);

        // Set the point that is behind the plane to the intesection point.
        if (d0 > 0.0f)
        {
            p0 = p;
        }
        else
        {
            p1 = p;
        }

        return true;
    }

}

}