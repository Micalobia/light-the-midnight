using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class LightSource : MonoBehaviour
{
    [SerializeField] [Range(0f, 360f)] public float FOV = 90f;
    [SerializeField] public float ViewDistance = 3f;
    [SerializeField] public int rayCount = 360;
    [SerializeField] public LayerMask LayerMask;

    private Vector3 origin;
    private float startingAngle => transform.eulerAngles.z + FOV / 2f;

    private Mesh mesh;
    private PolygonCollider2D polycol;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polycol = GetComponent<PolygonCollider2D>();
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
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;
        float angle = startingAngle;
        int vertexIndex = 1, triangleIndex = 0;
        float angleChange = FOV / rayCount;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vecAng = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 vertex;
            RaycastHit2D rayHit = Physics2D.Raycast(origin, vecAng, ViewDistance, LayerMask);
            vertex = (rayHit.collider == null) ? origin + vecAng * ViewDistance : (Vector3)rayHit.point; //Get where we hit if we hit, otherwise just put the point at our view distance
            //Debug.DrawRay(origin, vertex);
            //if (rayHit.collider != null) Debug.Log("Hit somethin'");
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
}
