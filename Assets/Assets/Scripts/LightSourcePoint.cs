using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class LightSourcePoint : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] [Range(0f, 360f)] public float FOV;
    [SerializeField] public float ViewDistance;
    [SerializeField] [Range(0, 540)] public int RayCount;
    [SerializeField] public LayerMask Layers;
    private static int reflectionLayer;

    private Mesh mesh;
    private PolygonCollider2D polycol;

    private void Reset()
    {
        FOV = 90f;
        ViewDistance = 3f;
        RayCount = 135;
        Layers = LayerMask.GetMask("Opaque", "Reflective");
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polycol = GetComponent<PolygonCollider2D>();
        reflectionLayer = LayerMask.NameToLayer("Reflection");
    }

    private void Update() => ConstructLight();

    private void ConstructLight()
    {
        int width = RayCount + 2;
        float count = RayCount + 1;
        float HalfFOV = FOV / 2;
        RayInfo[] rays = new RayInfo[width];
        Vector2[] vertices = new Vector2[width + 1];
        vertices[0] = Vector2.zero;
        for (int i = 0; i < width; ++i)
        {
            float angle = Mathf.LerpUnclamped(-HalfFOV, HalfFOV, i / count);
            rays[i] = Cast(angle);
            vertices[i + 1] = rays[i].hit;
        }
        polycol.enabled = false;
        polycol.pathCount = 1;
        polycol.SetPath(0, vertices);
        polycol.enabled = true;
        mesh.vertices = vertices.ToVector3();
        mesh.uv = vertices;
        int[] triangles = new int[vertices.Length * 3];
        for (int i = 2, trindex = 0; i <= width; i++, trindex += 3)
        {
            triangles[trindex] = 0;
            triangles[trindex + 1] = i;
            triangles[trindex + 2] = i - 1;
        }
        mesh.triangles = triangles;
    }

    private RayInfo Cast(float angle)
    {
        Vector2 angleVector = angle.Angle();
        RaycastHit2D rayHit = Physics2D.Raycast(transform.TransformPoint(Vector3.zero), transform.TransformDirection(angleVector), ViewDistance, Layers);
        if (rayHit)
        {
            bool reflected = rayHit.collider.gameObject.layer == reflectionLayer;
            return new RayInfo()
            {
                hit = transform.InverseTransformPoint(rayHit.point),
                distance = rayHit.distance,
                reflected = reflected,
                normal = reflected ? rayHit.normal : Vector2.up
            };
        }
        else return new RayInfo(angleVector * ViewDistance, ViewDistance, false, Vector2.up);
    }
}
