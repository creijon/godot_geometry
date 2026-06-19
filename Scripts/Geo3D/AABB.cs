using Godot;

namespace Geo3D
{
    public class AABB
    {
        public AABB(Vector3 centre, Vector3 extents)
        {
            this.centre = centre;
            this.extents = extents;
        }

        // By adding an unused bool to the constructor we initialise from a min and max value.
        // Validity not checked, but it doesn't matter since extents can be negative.
        public AABB(Vector3 min, Vector3 max, bool minMax)
        {
            SetMinMax(min, max);
        }

        public Vector3 Size => extents * 2.0f;

        public Vector3 Min
        {
            get { return centre - extents; }
            set { SetMinMax(value, Max); }
        }

        public Vector3 Max
        {
            get { return centre + extents; }
            set { SetMinMax(Min, value); }
        }

        public void SetMinMax(Vector3 min, Vector3 max)
        {
            extents = (max - min) * 0.5f;
            centre = min + extents;
        }

        public void Include(Vector3 p)
        {
            SetMinMax(Min.Min(p), Max.Max(p));
        }

        public Geo2D.Rect XY => new Geo2D.Rect(Util.XY(centre), Util.XY(extents));
        public Geo2D.Rect YZ => new Geo2D.Rect(Util.YZ(centre), Util.YZ(extents));
        public Geo2D.Rect ZX => new Geo2D.Rect(Util.ZX(centre), Util.ZX(extents));

        public Vector3 centre;
        public Vector3 extents;
    }
}
