using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class LightSource : MonoBehaviour
{
    [SerializeField] [Range(0f, 360f)] public float FOV;
    [SerializeField] public float ViewDistance;
    [SerializeField] public int RayCount = 135;
    [SerializeField] public LayerMask LayerMask;

    private Vector3 origin;
    private float startingAngle => transform.eulerAngles.z + FOV / 2f;
    private int _opaque;
    private int opaque => _opaque;
    private int _reflective;
    private int reflective => _reflective;

    private Mesh mesh;
    private PolygonCollider2D polycol;

    void Reset()
    {
        FOV = 90f;
        ViewDistance = 3f;
        RayCount = 360;
        LayerMask = LayerMask.GetMask("Opaque", "Reflective");
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polycol = GetComponent<PolygonCollider2D>();
        _reflective = LayerMask.GetMask("Reflective");
        _opaque = LayerMask.GetMask("Opaque");
    }

    void Update()
    {
        SetOrigin(transform.position);
        GenerateMesh();
        GeneratePolyCollider();
    }

    void GeneratePolyCollider()
    {
        polycol.pathCount = 1;
        polycol.enabled = false;
        polycol.SetPath(0, mesh.ToPolygon());
        polycol.enabled = true;
    }

    void SetOrigin(Vector3 origin) => this.origin = origin;

    Mesh GenerateMesh()
    {
        Vector3[] vertices = new Vector3[RayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[RayCount * 3];

        vertices[0] = Vector3.zero;
        float angle = startingAngle;
        int vertexIndex = 1, triangleIndex = 0;
        float angleChange = FOV / RayCount;
        for (int i = 0; i <= RayCount; i++)
        {
            Vector3 vecAng = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 vertex = CastRay(vecAng);
            //vertex = (rayHit.collider == null) ? origin + vecAng * ViewDistance : (Vector3)rayHit.point; //Get where we hit if we hit, otherwise just put the point at our view distance
            vertices[vertexIndex] = (vertex - transform.position).Rotate(-transform.eulerAngles.z);
            if (i != 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleChange;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        return mesh;
    }

    private Vector3 CastRay(Vector3 angle, float? _distance = null, Vector3? _origin = null, int depth = 4)
    {
        float distance = _distance ?? ViewDistance;
        Vector3 origin = _origin ?? this.origin;
        RaycastHit2D rayHit = Physics2D.Raycast(origin + angle, angle, distance, LayerMask);
        if(rayHit.collider)
        {
            int layer = 1 << rayHit.collider.gameObject.layer;
            Vector2 vec = rayHit.point;
            if (depth == 0) return vec;
            Debug.DrawLine(vec, vec + (Vector2)angle);
            if (layer == opaque) return vec;
            Vector3 normal = new Vector3(rayHit.normal.x, rayHit.normal.y);
            Vector3 reflectionAngle = angle - (2*Vector3.Dot(angle, normal)*normal);
            return CastRay(reflectionAngle, ViewDistance - rayHit.distance, vec, depth - 1);
        }
        return origin + angle * ViewDistance;
    }
}
