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
    public static float Angle(this Vector2 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

    /// <summary>
    /// Returns the vector as an angle, discards z
    /// </summary>
    /// <param name="v">The vector to convert</param>
    /// <returns>The angle in degrees</returns>
    public static float Angle(this Vector3 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

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

    /// <summary>
    /// Casts a Vector3 array to Vector2
    /// </summary>
    public static Vector2[] ToVector2(this Vector3[] v)
    {
        Vector2[] ret = new Vector2[v.Length];
        for (int i = 0; i < v.Length; i++) ret[i] = new Vector2(v[i].x, v[i].y);
        return ret;
    }
    /// <summary>
    /// Casts a Vector2 array to Vector3
    /// </summary>
    public static Vector3[] ToVector3(this Vector2[] v)
    {
        Vector3[] ret = new Vector3[v.Length];
        for (int i = 0; i < v.Length; i++) ret[i] = new Vector3(v[i].x, v[i].y);
        return ret;
    }

    /// <summary>
    /// Reflects the vector across the normal
    /// </summary>
    public static Vector2 Reflect(this Vector2 vector, Vector2 normal) => (2 * (Vector2.Dot(normal, vector)) * normal - vector);

    /// <summary>
    /// Reflects the vector across the normal
    /// </summary>
    public static Vector3 Reflect(this Vector3 vector, Vector3 normal) => -(2 * (Vector3.Dot(vector, normal)) * normal - vector);

    /// <summary>
    /// Sets this objects parent, and resets the local scale and position
    /// </summary>
    /// <param name="child">The child</param>
    /// <param name="parent">The parent</param>
    public static void SetParentClean(this Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    /// <summary>
    /// Returns the vector with all coordinates equal to 1 divided by their value, or 0 if it's 0
    /// </summary>
    /// <param name="vec">The vector</param>
    /// <returns>The inverted vector</returns>
    public static Vector3 Inverse(this Vector3 vec) => new Vector3(vec.x == 0f ? 0f : 1f / vec.x, vec.y == 0f ? 0f : 1f / vec.y, vec.z == 0f ? 0f : 1f / vec.z);

    /// <summary>
    /// Returns the vector with all coordinates equal to 1 divided by their value, or 0 if it's 0
    /// </summary>
    /// <param name="vec">The vector</param>
    /// <returns>The inverted vector</returns>
    public static Vector2 Inverse(this Vector2 vec) => new Vector2(vec.x == 0f ? 0f : 1f / vec.x, vec.y == 0f ? 0f : 1f / vec.y);
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