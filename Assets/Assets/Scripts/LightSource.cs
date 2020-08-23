using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
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
    private MeshCollider meshcol;

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
        //polycol = GetComponent<PolygonCollider2D>();
        meshcol = GetComponent<MeshCollider>();
        _reflective = LayerMask.GetMask("Reflective");
        _opaque = LayerMask.GetMask("Opaque");
    }

    void Update()
    {
        SetOrigin(transform.position);
       meshcol.sharedMesh = GenerateMesh();
        //GeneratePolyCollider();
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
        List<Vector3?> vertices = new List<Vector3?>(RayCount * 4 + 5);
        List<int> triangles = new List<int>();

        vertices.Add(origin);
        float angle = startingAngle;
        int vertexIndex = 1, triangleIndex = 0;
        float angleChange = FOV / RayCount;

        Vector3?[][] verts = new Vector3?[RayCount + 2][];
        for (int i = 0; i <= RayCount; ++i)
        {
            Vector3 ang = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            angle -= angleChange;
            Vector3?[] cast = CastRay(ang);
            verts[i] = cast;
        }
        //for (int i = 0; i < RayCount; i++)
        //{
        //    Vector3 vertex = verts[0][i+1] ?? Vector3.zero;
        //    vertices.Add((vertex - transform.position).Rotate(-transform.eulerAngles.z));
        //    if (i != 0)
        //    {
        //        triangles.Add(0);
        //        triangles.Add(vertexIndex - 1);
        //        triangles.Add(vertexIndex);
        //        triangleIndex += 3;
        //    }
        //    vertexIndex++;
        //}
        for (int i = 0; i < 4; ++i)
            for (int j = 0; j <= RayCount; ++j)
                vertices.Add(verts[j][i]);
        for (int i = 2; i <= RayCount; ++i)
        {
            triangles.Add(0);
            triangles.Add(i - 1);
            triangles.Add(i);
        }
        int width = RayCount + 1;
        for (int i = 1; i < 4; ++i)
        {
            for (int j = 0; j <= RayCount; ++j)
            {
                int refl = i * width + 1;
                if (!vertices[refl].HasValue) continue;
                int prerefl = refl - 1;
                if (!vertices[prerefl].HasValue) continue;
                int main = (i - 1) * width + 1;
                int premain = main - 1;
                triangles.Add(premain);
                triangles.Add(main);
                triangles.Add(prerefl);
                triangles.Add(main);
                triangles.Add(refl);
                triangles.Add(prerefl);
            }
        }
        for (int i = 0; i < vertices.Count; ++i)
        {
            if (!vertices[i].HasValue)
            {
                vertices.RemoveAt(i--);
                for (int j = 0; j < triangles.Count; ++j)
                    if (triangles[j] > i) --triangles[j];
            }
            else
            {
                vertices[i] = (vertices[i].Value - transform.position).Rotate(-transform.eulerAngles.z);
            }
        }


        //for (int i = 0; i <= RayCount; ++i)
        //{
        //    Vector3 vecAng = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        //    Vector3 vertex = CastRay(vecAng);
        //    //vertex = (rayHit.collider == null) ? origin + vecAng * ViewDistance : (Vector3)rayHit.point; //Get where we hit if we hit, otherwise just put the point at our view distance
        //    vertices[vertexIndex] = (vertex - transform.position).Rotate(-transform.eulerAngles.z);
        //    if (i != 0)
        //    {
        //        triangles[triangleIndex] = 0;
        //        triangles[triangleIndex + 1] = vertexIndex - 1;
        //        triangles[triangleIndex + 2] = vertexIndex;
        //        triangleIndex += 3;
        //    }
        //    vertexIndex++;
        //    angle -= angleChange;
        //}

        // Vector2[] uv = new Vector2[vertices.Count];

        var filter = GetComponent<MeshFilter>();
        mesh.triangles = triangles.ToArray();
        mesh.vertices = (from v in vertices select new Vector3(v.Value.x,v.Value.y)).ToArray();
        mesh.uv = new Vector2[mesh.vertices.Length];
        return mesh;
    }

    private Vector3?[] CastRay(Vector3 angle, float? _distance = null, Vector3? _origin = null, int depth = 4)
    {
        float distance = _distance ?? ViewDistance;
        Vector3 origin = _origin ?? this.origin;
        RaycastHit2D rayHit = Physics2D.Raycast(origin, angle, distance, LayerMask);
        Vector3?[] ret = new Vector3?[depth];
        if (rayHit)
        {
            int layer = 1 << rayHit.collider.gameObject.layer;
            ret[0] = rayHit.point;
            if (depth == 1 || layer == opaque) return ret;
            Vector3 norm = rayHit.normal;
            Vector3 refAng = angle - 2 * Vector3.Dot(angle, norm) * norm;
            Vector3?[] cast = CastRay(refAng, distance - rayHit.distance - 1, ret[0]+refAng, depth - 1);
            for (int i = 0; i < cast.Length; i++) ret[i + 1] = cast[i];
            return ret;
        }
        ret[0] = origin + angle * distance;
        return ret;
    }
}
