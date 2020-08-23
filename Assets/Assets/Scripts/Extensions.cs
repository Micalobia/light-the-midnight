using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VectorExtension
{
    /// <summary>
    /// Rotates the point about the origin
    /// </summary>
    /// <param name="v">The point to rotate</param>
    /// <param name="angle">The angle in degrees</param>
    /// <returns>The rotated point</returns>
    public static Vector2 Rotate(this Vector2 v, float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float x = v.x;
        float y = v.y;
        v.x = x * cos - y * sin;
        v.y = y * cos + x * sin;
        return v;
    }

    /// <summary>
    /// Rotates the point about the origin
    /// </summary>
    /// <param name="v">The point to rotate</param>
    /// <param name="origin">The origin</param>
    /// <param name="angle">The angle in degrees</param>
    /// <returns>The rotated point</returns>
    public static Vector2 Rotate(this Vector2 v, Vector2 origin, float angle) => (v - origin).Rotate(angle) + origin;

    /// <summary>
    /// Rotates the point about the origin
    /// </summary>
    /// <param name="v">The point to rotate</param>
    /// <param name="angle">The angle in degrees</param>
    /// <returns>The rotated point</returns>
    public static Vector3 Rotate(this Vector3 v, float angle) => ((Vector2)v).Rotate(angle);

    /// <summary>
    /// Rotates the point about the origin
    /// </summary>
    /// <param name="v">The point to rotate</param>
    /// <param name="origin">The origin</param>
    /// <param name="angle">The angle in degrees</param>
    /// <returns>The rotated point</returns>
    public static Vector3 Rotate(this Vector3 v, Vector3 origin, float angle) => ((Vector2)v).Rotate(origin, angle);

    /// <summary>
    /// Returns the vector as an angle
    /// </summary>
    /// <param name="v">The vector to convert</param>
    /// <returns>The angle in degrees</returns>
    public static float Angle(this Vector2 v) => Mathf.Atan2(v.y, v.x)*Mathf.Rad2Deg;

    /// <summary>
    /// Returns the vector as an angle, discards z
    /// </summary>
    /// <param name="v">The vector to convert</param>
    /// <returns>The angle in degrees</returns>
    public static float Angle(this Vector3 v) => Mathf.Atan2(v.y, v.x)*Mathf.Rad2Deg;

    /// <summary>
    /// Returns the angle as a vector
    /// </summary>
    /// <param name="angle">The angle to convert</param>
    /// <returns>The normalizes vector</returns>
    public static Vector2 Angle(this float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public static Vector2[] ToVector2(this Vector3[] v)
    {
        Vector2[] ret = new Vector2[v.Length];
        for (int i = 0; i < v.Length; i++) ret[i] = new Vector2(v[i].x, v[i].y);
        return ret;
    }
    public static Vector3[] ToVector3(this Vector2[] v)
    {
        Vector3[] ret = new Vector3[v.Length];
        for (int i = 0; i < v.Length; i++) ret[i] = new Vector3(v[i].x, v[i].y);
        return ret;
    }

    /// <summary>
    /// Turns a mesh into a polygon by droping the z
    /// </summary>
    /// <param name="mesh">The mesh to convert</param>
    /// <returns>The list of points representing the polygon</returns>
    public static Vector2[] ToPolygon(this Mesh mesh) => (from v in mesh.vertices select new Vector2(v.x, v.y)).ToArray();
    public static Vector2[] ToPolygonOld(this Mesh mesh)
    {
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        List<Edge> edges = new List<Edge>(tris.Length);
        for (int i = 0; i < tris.Length; i += 3)
        {
            int v0 = tris[i];
            int v1 = tris[i + 1];
            int v2 = tris[i + 2];
            edges.Add(new Edge(v0, v1, i));
            edges.Add(new Edge(v1, v2, i));
            edges.Add(new Edge(v2, v0, i));
        }
        for (int i = edges.Count - 1; i > 0; i--)
            for (int j = i - 1; j >= 0; j--)
            {
                if (edges[i].v0 == edges[j].v1 && edges[i].v1 == edges[j].v0)
                {
                    edges.RemoveAt(i);
                    edges.RemoveAt(j);
                    i--;
                    break;
                }
            }
        for (int i = 0; i < edges.Count - 2; i++)
        {
            Edge E = edges[i];
            for (int j = i + 1; j < edges.Count; j++)
            {
                Edge a = edges[j];
                if (E.v1 == a.v0)
                {
                    if (j == i + 1) break;
                    edges[j] = edges[i + 1];
                    edges[i + 1] = a;
                    break;
                }
            }
        }
        int[] vecs = new int[edges.Count];
        for (int i = 0; i < vecs.Length; i++) vecs[i] = edges[i].v0;
        int[] unique = vecs.Distinct().ToArray();
        Vector2[] ret = new Vector2[vecs.Length];
        for (int i = 0; i < unique.Length; i++) ret[i] = new Vector2(verts[unique[i]].x, verts[unique[i]].y);
        return ret;
    }
}

public struct Edge
{
    public int v0;
    public int v1;
    public int tri;
    public Edge(int v0, int v1, int tri)
    {
        this.v0 = v0;
        this.v1 = v1;
        this.tri = tri;
    }
}