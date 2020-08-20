using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLightSource : MonoBehaviour
{
    [SerializeField] public float FOV = 90f;
    [SerializeField] public float ViewDistance = 3f;
    //[SerializeField] public float Angle = 0f;
    [SerializeField] public LayerMask LayerMask;

    private Vector3 origin = Vector3.zero;
    private int rayCount = 360;
    private float startingAngle => transform.eulerAngles.z + FOV / 2f;

    private Mesh mesh;
    public Mesh Mesh => mesh;

    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir.Normalize();
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        SetOrigin(transform.position);
    }

    void LateUpdate()
    {
        GenerateMesh();
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
        int mask = LayerMask.GetMask("Level");
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vecAng = GetVectorFromAngle(angle);
            Vector3 vertex;
            RaycastHit2D rayHit = Physics2D.Raycast(origin, vecAng, ViewDistance, LayerMask); //Should only hit colliders with the Level tag, doesn't hit anything though
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
