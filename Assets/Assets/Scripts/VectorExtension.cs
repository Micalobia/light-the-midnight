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
        float sin = Mathf.Sin(angle*Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle*Mathf.Deg2Rad);
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

}

