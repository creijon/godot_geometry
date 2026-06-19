using Godot;

namespace Geo3D
{
    public static class Intersect
    {
        public static bool Test(Vector3 p, AABB aabb)
        {
            var cr = p - aabb.centre;

            if (Mathf.Abs(cr.X) > aabb.extents.X) return false;
            if (Mathf.Abs(cr.Y) > aabb.extents.Y) return false;
            if (Mathf.Abs(cr.Z) > aabb.extents.Z) return false;

            return true;
        }

        public static bool Test(AABB aabb1, AABB aabb2)
        {
            var cr = aabb1.centre - aabb2.centre;
            var e = aabb1.extents + aabb2.extents;

            if (Mathf.Abs(cr.X) > e.X) return false;
            if (Mathf.Abs(cr.Y) > e.Y) return false;
            if (Mathf.Abs(cr.Z) > e.Z) return false;

            return true;
        }

        public static bool Test(Ray ray, AABB aabb, out float t)
        {
            var invDir = Util.Reciprocal(ray.dir);
            var rmin = (aabb.Min - ray.origin) * invDir;
            var rmax = (aabb.Max - ray.origin) * invDir;
            var tmax = Util.MinCoefficient(rmin.Max(rmax));
            t = tmax;
            if (tmax < 0.0f) return false;

            var tmin = Util.MaxCoefficient(rmin.Min(rmax));
            if (tmin > tmax) return false;

            t = tmin;

            return true;
        }

        public static bool Test(Edge edge, AABB aabb)
        {
            var ha = edge.Axis * 0.5f;
            var cr = edge.Centre - aabb.centre;
            var e = aabb.extents;
            var ahax = Mathf.Abs(ha.X);          // Exploiting symmetry
            var ahay = Mathf.Abs(ha.Y);
            var ahaz = Mathf.Abs(ha.Z);

            if (Mathf.Abs(cr.X) > e.X + ahax) return false;
            if (Mathf.Abs(cr.Y) > e.Y + ahay) return false;
            if (Mathf.Abs(cr.Z) > e.Z + ahaz) return false;
            if (Mathf.Abs(ha.Y * cr.Z - ha.Z * cr.Y) > e.Y * ahaz + e.Z * ahay + Mathf.Epsilon) return false;
            if (Mathf.Abs(ha.Z * cr.X - ha.X * cr.Z) > e.Z * ahax + e.X * ahaz + Mathf.Epsilon) return false;
            if (Mathf.Abs(ha.X * cr.Y - ha.Y * cr.X) > e.X * ahay + e.Y * ahax + Mathf.Epsilon) return false;

            return true;
        }

        // Adapted from Moller-Trumbore solution:
        // https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
        public static bool Test(Ray ray, Triangle tri, out float t)
        {
            var e1 = tri.v1 - tri.v0;
            var e2 = tri.v2 - tri.v0;
            var P = ray.dir.Cross(e2);
            var det = e1.Dot(P);
            t = 0.0f;

            if (det > -Mathf.Epsilon && det < Mathf.Epsilon) return false;

            float invDet = 1.0f / det;
            var T = ray.origin - tri.v0;
            var u = T.Dot(P) * invDet;
            if (u < 0.0f || u > 1.0f) return false;

            var Q = T.Cross(e1);
            var v = ray.dir.Dot(Q * invDet);
            if (v < 0.0f || u + v > 1.0f) return false;

            t = e2.Dot(Q) * invDet;
            if (t > Mathf.Epsilon) return true;

            return false;
        }

        public static bool Test(Edge edge, Triangle tri, out float t)
        {
            var d = edge.Axis;
            var ld = d.Length();
            var dir = d / ld;
            
            if (Test(new Ray(edge.v0, dir), tri, out t))
            {
                if (t <= ld)
                {
                    t /= ld;
                    return true;
                }
            }

            return false;
        }

        public static bool Test(Vector3 p, Triangle tri)
        {
            var e1 = tri.v2 - tri.v0;
            var e0 = tri.v1 - tri.v0;
            var eP = p - tri.v0;

            float dot01 = e0.Dot(e1);
            float dot0P = e0.Dot(eP);
            float dot11 = e1.Dot(e1);
            float dot1P = e1.Dot(eP);

            // Test edge1
            float u = dot11 * dot0P - dot01 * dot1P;
            if (u < 0.0f) return false;

            // Test edge0
            float dot00 = e0.Dot(e0);
            float v = dot00 * dot1P - dot01 * dot0P;
            if (v < 0.0f) return false;

            float denom = dot00 * dot11 - dot01 * dot01;
            if (denom < u + v) return false;

            return true;
        }

        public static bool Test(Plane plane, AABB aabb)
        {
            var r = aabb.extents.Dot(Util.Abs(plane.n));
            var s = plane.SignedDistance(aabb.centre);

            return Mathf.Abs(s) <= r;
        }

        public static bool Test(Edge edge, Plane plane)
        {
            var d0 = plane.SignedDistance(edge.v0);
            var d1 = plane.SignedDistance(edge.v1);

            if (d0 * d1 > 0.0f) return false;

            return true;
        }

        public static bool Test(Edge edge, Plane plane, out float t)
        {
            t = 0.0f;
            var d0 = plane.SignedDistance(edge.v0);
            var d1 = plane.SignedDistance(edge.v1);

            if (d0 * d1 > 0.0f) return false;

            t = d0 / (d0 - d1);

            return true;
        }

        public static bool Test(Triangle tri, AABB aabb)
        {
            // Early out if the AABB of the triangle is disjoint with the AABB.
            if (!Test(tri.Bounds(), aabb)) return false;

            return TestNoBB(tri, aabb);
        }

        // Adapted from Schwarz-Seidel triangle-box intersection:
        // https://michael-schwarz.com/research/publ/2010/vox/
        // Provided for performance comparisons.
        public static bool TestSS(Triangle tri, AABB aabb)
        {
            var n = tri.Cross;
            var r = aabb.extents.Dot(Util.Abs(n));
            var s = n.Dot(aabb.centre - tri.v0);

            if (Mathf.Abs(s) > r) return false;

            if (!Geo2D.Intersect.Test(tri.XY, aabb.XY)) return false;
            if (!Geo2D.Intersect.Test(tri.YZ, aabb.YZ)) return false;
            if (!Geo2D.Intersect.Test(tri.ZX, aabb.ZX)) return false;

            return true;
        }

        public static bool TestNoBB(Triangle tri, AABB aabb)
        {
            // Test three triangle edges against box.
            if (Test(tri.Edge0, aabb)) return true;
            if (Test(tri.Edge1, aabb)) return true;
            if (Test(tri.Edge2, aabb)) return true;

            // If none of the edges of a degenerate triangle intersect then don't test any further.
            var n = tri.Cross;
            if (n.Dot(n) < Mathf.Epsilon) return false;
    
            // Test if plane of triangle intersects the box.
            var r = aabb.extents.Dot(Util.Abs(n));
            var s = n.Dot(aabb.centre - tri.v0);
            if (Mathf.Abs(s) > r) return false;

            // Test the four internal diagonals of the box against the triangle.
            // This catches the situations where the middle of the triangle is intersected
            // by the box but not any of the edges.
            float t;
            var min = aabb.Min;
            var max = aabb.Max;
            var axis0 = max - min;
            var invLength = 1.0f / axis0.Length();

            if (Test(new Ray(min, axis0 * invLength), tri, out t))
            {
                if (t * invLength <= 1.0f) return true;
            }

            var i1a = new Vector3(max.X, min.Y, min.Z);
            var i1b = new Vector3(min.X, max.Y, max.Z);
            var axis1 = i1b - i1a;

            if (Test(new Ray(i1a, axis1 * invLength), tri, out t))
            {
                if (t * invLength <= 1.0f) return true;
            }

            var i2a = new Vector3(min.X, max.Y, min.Z);
            var i2b = new Vector3(max.X, min.Y, max.Z);
            var axis2 = i2b - i2a;

            if (Test(new Ray(i2a, axis2 * invLength), tri, out t))
            {
                if (t * invLength <= 1.0f) return true;
            }

            var i3a = new Vector3(max.X, max.Y, min.Z);
            var i3b = new Vector3(min.X, min.Y, max.Z);
            var axis3 = i3b - i3a;

            if (Test(new Ray(i3a, axis3 * invLength), tri, out t))
            {
                if (t * invLength <= 1.0f) return true;
            }

            return false;
        }
    }
}
