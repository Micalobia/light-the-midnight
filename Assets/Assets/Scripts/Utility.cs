using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class Utility
{
    //public static Vector2[] PolygonUnion(Vector2[] a, Vector2[] b)
    //{
    //    List<Line> la = new List<Line>(a.Length);
    //    List<Line> lb = new List<Line>(b.Length);
    //    for (int i = 1; i < a.Length; i++) la.Add(new Line(a[i - 1], a[i]));
    //    for (int i = 1; i < b.Length; i++) lb.Add(new Line(b[i - 1], b[i]));
    //    la.Add(new Line(a.Last(), a[0]));
    //    lb.Add(new Line(b.Last(), b[0]));
    //    for(int i = 0; i < la.Count; i++)
    //        for(int j = 0; j < lb.Count; j++)
    //        {
    //            Vector2? _vec;
    //            _vec = Line.Intersection(la[i], lb[i]);
    //            if (_vec == null) continue;
    //            Vector2 vec = (Vector2)_vec;

    //        }
    //}
}

public struct Line
{
    public Vector2 u;
    public Vector2 v;
    public bool Vertical => u.x == v.x;
    public bool Horizontal => u.y == v.y;
    public Line(Vector2 a, Vector2 b)
    {
        u = a;
        v = b;
    }
    public Line Swap() => new Line(v, u);
    public static bool Same(Line a, Line b) => a == b || a == b.Swap();
    public static bool operator ==(Line a, Line b) => a.u == b.u && a.v == b.v;
    public static bool operator !=(Line a, Line b) => !(a == b);
    public static Vector2? Intersection(Line a, Line b)
    {
        float p0_x = a.u.x, p0_y = a.u.y;
        float p1_x = a.v.x, p1_y = a.v.y;
        float p2_x = b.u.x, p2_y = b.u.y;
        float p3_x = b.v.x, p3_y = b.v.y;
        float s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, s_numer, t_numer, denom, t;
        s10_x = p1_x - p0_x;
        s10_y = p1_y - p0_y;
        s32_x = p3_x - p2_x;
        s32_y = p3_y - p2_y;

        denom = s10_x * s32_y - s32_x * s10_y;
        if (denom == 0)
            return Vector2.positiveInfinity; // Collinear
        bool denomPositive = denom > 0;

        s02_x = p0_x - p2_x;
        s02_y = p0_y - p2_y;
        s_numer = s10_x * s02_y - s10_y * s02_x;
        if ((s_numer < 0) == denomPositive)
            return null; // No collision

        t_numer = s32_x * s02_y - s32_y * s02_x;
        if ((t_numer < 0) == denomPositive)
            return null; // No collision

        if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
            return null; // No collision
                         // Collision detected
        t = t_numer / denom;
        float i_x = p0_x + (t * s10_x);
        float i_y = p0_y + (t * s10_y);

        return new Vector2(i_x, i_y);
    }
}

