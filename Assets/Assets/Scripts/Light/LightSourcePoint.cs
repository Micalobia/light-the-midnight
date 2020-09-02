using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LightSourcePoint : MonoBehaviour, ILightSource
{
    [Header("Other")]
    [SerializeField] [Range(0f, 360f)] float FOV;
    [SerializeField] float ViewDistance;
    [SerializeField] [Range(0, 540)] public int RayCount;
    [SerializeField] [Tooltip("Which layers the light can get blocked by")] LayerMask Layers;
    [SerializeField] [Tooltip("Which object to create when creating a reflection, should be the Reflection prefab")] GameObject Reflection;
    [SerializeField] [Tooltip("How many reflections are allowed")] [Range(1, 10)] int Depth;
    [SerializeField] bool StartOn;

    private static int reflectionLayer;
    private List<GameObject> reflections;
    private List<GameObject> reflectionsOld;

    private Mesh mesh;
    private PolygonCollider2D polyCol;
    private MeshRenderer meshRenderer;

    public event OnLightTriggerDelegate OnLightTrigger;
    private bool _on;

    public bool TurnedOn
    {
        get => _on;
        set
        {
            _on = value;
            polyCol.enabled = value;
            meshRenderer.enabled = value;
        }
    }
    public Vector2 WorldCenter => transform.parent.TransformPoint(transform.position);

    private void Reset()
    {
        FOV = 60f;
        ViewDistance = 4f;
        RayCount = 90;
        Layers = LayerMask.GetMask("Opaque", "Reflective");
        Depth = 4;
        StartOn = true;
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polyCol = GetComponent<PolygonCollider2D>();
        reflections = new List<GameObject>();
        reflectionsOld = new List<GameObject>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = "UI";
        TurnedOn = StartOn;
        transform.lossyScale.Set(1, 1, 1);
    }

    private void Update()
    {
        for (int i = 0; i < reflectionsOld.Count; i++)
        {
            foreach (Transform t in reflectionsOld[i].transform)
            {
                t.parent = transform;
                reflections.Add(t.gameObject);
            }
            Destroy(reflectionsOld[i]);
        }
        for (int i = 0; i < reflectionsOld.Count; i++) Destroy(reflectionsOld[i]);
        reflectionsOld = reflections.ToList();
        reflections = new List<GameObject>();
        if (TurnedOn)
        {
            ConstructLight();
        }
        else
        {
            mesh.Clear();
            mesh.vertices = new Vector3[0];
            mesh.uv = new Vector2[0];
            mesh.triangles = new int[0];
            mesh.RecalculateBounds();
            polyCol.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) => OnLightTrigger?.Invoke(ref collision);

    private void ConstructLight()
    {
        reflectionLayer = LayerMask.NameToLayer("Reflective");
        int width = RayCount + 2;
        float count = RayCount + 1;
        float HalfFOV = FOV / 2;
        RayInfo[] rays = new RayInfo[width];
        Vector2[] vertices = new Vector2[width + 1];
        List<List<RayInfo>> rayInfos = new List<List<RayInfo>>(width / 4);
        vertices[0] = Vector2.zero;
        for (int i = 0; i < width; ++i)
        {
            float angle = Mathf.LerpUnclamped(-HalfFOV, HalfFOV, i / count);
            rays[i] = Cast(angle);
            if (rays[i].reflected)
            {
                int lastList = rayInfos.Count - 1;
                if (rayInfos.Count > 0 && rayInfos[lastList][rayInfos[lastList].Count - 1].normal == rays[i].normal) rayInfos[lastList].Add(rays[i]);
                else
                {
                    rayInfos.Add(new List<RayInfo>());
                    rayInfos[lastList + 1].Add(rays[i]);
                }
            }
            vertices[i + 1] = rays[i].hit;
        }
        polyCol.enabled = false;
        polyCol.pathCount = 1;
        polyCol.SetPath(0, vertices);
        polyCol.enabled = TurnedOn;
        mesh.Clear();
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
        mesh.RecalculateBounds();
        if (Depth == 1) return;
        for (int i = 0; i < rayInfos.Count; i++)
        {
            if (rayInfos[i].Count < 2)
                rayInfos.RemoveAt(i--);
        }
        for (int i = 0; i < rayInfos.Count; i++)
        {
            int last = rayInfos[i].Count - 1;
            GameObject cur = Instantiate(Reflection);
            LightSourceLine source = cur.GetComponent<LightSourceLine>();
            cur.transform.position = transform.position;
            cur.transform.rotation = Quaternion.identity;
            source.Angle0 = cur.transform.InverseTransformDirection(rayInfos[i][0].angle).Angle();
            source.Angle1 = cur.transform.InverseTransformDirection(rayInfos[i][last].angle).Angle();
            source.Vector0 = cur.transform.InverseTransformPoint(transform.TransformPoint(rayInfos[i][0].hit));
            source.Vector1 = cur.transform.InverseTransformPoint(transform.TransformPoint(rayInfos[i][last].hit));
            source.RayCount = RayCount;
            source.ViewDistance0 = ViewDistance;// - rayInfos[i][0].distance;
            source.ViewDistance1 = ViewDistance;// - rayInfos[i][last].distance;
            source.Reflection = Reflection;
            source.Layers = Layers;
            source.transform.parent = transform;
            source.Depth = Depth - 1;
            cur.SetActive(true);
            reflections.Add(cur);
        }
    }

    private RayInfo Cast(float angle)
    {
        Vector2 angleVector = angle.Angle();
        Vector2 transVector = transform.TransformDirection(angleVector);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.TransformPoint(Vector3.zero) + (Vector3)(transVector * 0.5f), transVector, ViewDistance - 0.5f, Layers);
        if (rayHit)
        {
            bool reflected = rayHit.collider.gameObject.layer == reflectionLayer;
            Vector2 normal = reflected ? transform.InverseTransformDirection(rayHit.normal) : Vector3.up;
            return new RayInfo()
            {
                hit = transform.InverseTransformPoint(rayHit.point),
                normal = normal,
                angle = (-transVector).Reflect(rayHit.normal),
                distance = rayHit.distance,
                reflected = reflected,
                lerp = 1f
            };
        }
        else return new RayInfo(angleVector * ViewDistance / transform.lossyScale.x, Vector2.up, angleVector, ViewDistance, false);
    }
}
