using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
public class LightSourceLine : MonoBehaviour
{
    [Header("Side 1")]
    [SerializeField] public Vector3 vector0;
    [SerializeField] [Range(0, 360)] public float angle0;
    [Header("Side 2")]
    [SerializeField] public Vector3 vector1;
    [SerializeField] [Range(0, 360)] public float angle1;
    [Header("Other")]
    [SerializeField] [Range(0, 540)] public int RayCount;
    [SerializeField] public float ViewDistance;
    [SerializeField] public LayerMask Layers;
    private Vector2 root => new Vector2(transform.position.x, transform.position.y);
    private static int reflectionLayer;

    private PolygonCollider2D polygonCollider;
    private Mesh mesh;

    public LightSourceLine(Vector3 vector0, Vector3 vector1, float angle0, float angle1, int RayCount, float ViewDistance)
    {
        this.vector0 = vector0;
        this.vector1 = vector1;
        this.angle0 = angle0;
        this.angle1 = angle1;
        this.RayCount = RayCount;
        this.ViewDistance = ViewDistance;
    }

    private void Reset()
    {
        vector0 = Vector3.zero;
        vector1 = Vector3.zero;
        angle0 = 0f;
        angle1 = 0f;
        RayCount = 180;
        ViewDistance = 4f;
        Layers = 0;
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        reflectionLayer = LayerMask.NameToLayer("Reflective");
    }

    private void Update() => ConstructLight();

    private void ConstructLight()
    {
        float z = transform.eulerAngles.z;
        float count = RayCount + 1;
        int width = RayCount + 2;
        RayInfo[] rays = new RayInfo[width];
        Vector2[] vertices = new Vector2[width * 2];
        for (int i = 0; i < width; i++)
        {
            float t = i / count;
            Vector3 b = BaseLerp(t);
            Vector2 origin = new Vector2(b.x, b.y);
            float angle = b.z;
            rays[i] = Cast(origin, angle);
            vertices[i] = origin;
            vertices[vertices.Length - i - 1] = rays[i].hit;
        }
        polygonCollider.enabled = false;
        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, vertices);
        polygonCollider.enabled = true;
        mesh.vertices = vertices.ToVector3();
        mesh.uv = new Vector2[vertices.Length];
        int[] triangles = new int[vertices.Length * 3];
        for (int i = 1; i < width; i++)
        {
            int main = i;
            int premain = main - 1;
            int cast = 2 * width - i - 1;
            int precast = cast + 1;
            int trindex = premain * 6;
            triangles[trindex] = cast;
            triangles[trindex + 1] = main;
            triangles[trindex + 2] = premain;
            triangles[trindex + 3] = premain;
            triangles[trindex + 4] = precast;
            triangles[trindex + 5] = cast;
        }
        mesh.triangles = triangles;
    }

    private Vector3 BaseLerp(float t) =>
        new Vector3(
            Mathf.LerpUnclamped(vector0.x, vector1.x, t),
            Mathf.LerpUnclamped(vector0.y, vector1.y, t),
            Mathf.LerpAngle(angle0, angle1, t)
        );

    private RayInfo Cast(Vector2 origin, float angle)
    {
        Vector2 angleVector = angle.Angle();
        RaycastHit2D rayHit = Physics2D.Raycast(transform.TransformPoint(origin), transform.TransformDirection(angleVector), ViewDistance, Layers);
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
        else return new RayInfo(origin + angleVector * ViewDistance, ViewDistance, false, Vector2.up);
    }
}
