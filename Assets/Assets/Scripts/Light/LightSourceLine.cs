using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshRenderer))]
public class LightSourceLine : MonoBehaviour, ILightSource
{
    [Header("Side 1")]
    [SerializeField] public Vector3 Vector0;
    [SerializeField] [Range(-180, 180)] public float Angle0;
    [SerializeField] public float ViewDistance0;
    [Header("Side 2")]
    [SerializeField] public Vector3 Vector1;
    [SerializeField] [Range(-180, 180)] public float Angle1;
    [SerializeField] public float ViewDistance1;
    [Header("Other")]
    [SerializeField] [Range(0, 540)] public int RayCount;
    [SerializeField] [Tooltip("Which layers the light can get blocked by")] public LayerMask Layers;
    [SerializeField] [Tooltip("Which object to create when creating a reflection, should be the Reflection prefab")] public GameObject Reflection;
    [SerializeField] [Tooltip("How many reflections are allowed")] [Range(1, 10)] public int Depth;
    [SerializeField] public bool StartOn;
    [SerializeField] private bool Interactable;

    private Vector2 root => new Vector2(transform.position.x, transform.position.y);
    public Vector2 WorldCenter => transform.TransformPoint(transform.position);

    public event OnLightTriggerDelegate OnLightTrigger;
    public bool TurnedOn
    {
        get => _on;
        set
        {
            _on = value;
            polyCol.enabled = _on;
            meshRenderer.enabled = _on;
        }
    }

    public bool UseInteract { get => Interactable; set => Interactable = value; }
    public InteractReceiver interactReceiver { get; set; }

    private static int reflectionLayer;
    private List<GameObject> reflections;
    private List<GameObject> reflectionsOld;
    private bool _on;

    private PolygonCollider2D polyCol;
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private void Reset()
    {
        Vector0 = Vector3.zero;
        Vector1 = Vector3.zero;
        Angle0 = 0f;
        Angle1 = 0f;
        RayCount = 180;
        ViewDistance0 = 4f;
        ViewDistance1 = 4f;
        Layers = LayerMask.GetMask("Opaque", "Reflective");
        Reflection = null;
        Depth = 4;
        StartOn = true;
        Interactable = false;
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        polyCol = GetComponent<PolygonCollider2D>();
        polyCol.isTrigger = true;
        reflections = new List<GameObject>();
        reflectionsOld = new List<GameObject>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = "UI";
        TurnedOn = StartOn;
        if (UseInteract)
        {
            interactReceiver = GetComponent<InteractReceiver>();
            interactReceiver.OnInteract += () => TurnedOn = !TurnedOn;
        }
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
        //reflectionsOld = reflections.ToList();
        //reflections = new List<GameObject>();
        //ConstructLight();
    }

    private void OnTriggerEnter2D(Collider2D collision) => OnLightTrigger?.Invoke(ref collision);

    private void ConstructLight()
    {
        reflectionLayer = LayerMask.NameToLayer("Reflective");
        float z = transform.eulerAngles.z;
        float count = RayCount + 1;
        int width = RayCount + 2;
        RayInfo[] rays = new RayInfo[width];
        Vector2[] vertices = new Vector2[width * 2];
        List<List<RayInfo>> rayInfos = new List<List<RayInfo>>(width / 4);
        for (int i = 0; i < width; i++)
        {
            float t = i / count;
            Vector3 b = BaseLerp(t);
            Vector2 origin = new Vector2(b.x, b.y);
            float angle = b.z;
            rays[i] = Cast(origin, angle, t);
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
            vertices[i] = origin;
            vertices[vertices.Length - i - 1] = rays[i].hit;
        }
        polyCol.enabled = false;
        polyCol.pathCount = 1;
        polyCol.SetPath(0, vertices);
        polyCol.enabled = TurnedOn;
        mesh.Clear();
        mesh.vertices = vertices.ToVector3();
        mesh.uv = vertices;
        int[] triangles = new int[vertices.Length * 3];
        bool c = clockwise(vertices[0], vertices[1], vertices[vertices.Length - 2]);
        if (c)
        {
            for (int i = 1; i < width; i++)
            {
                int main = i;
                int premain = main - 1;
                int cast = 2 * width - i - 1;
                int precast = cast + 1;
                int trindex = premain * 6;
                triangles[trindex] = premain;
                triangles[trindex + 1] = main;
                triangles[trindex + 2] = cast;
                triangles[trindex + 3] = cast;
                triangles[trindex + 4] = precast;
                triangles[trindex + 5] = premain;
            }
        }
        else
        {
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
        }
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        if (Depth == 1) return;
        for (int i = 0; i < rayInfos.Count; i++)
            if (rayInfos[i].Count < 2)
                rayInfos.RemoveAt(i--);
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
            source.ViewDistance0 = Mathf.LerpUnclamped(ViewDistance0, ViewDistance1, rayInfos[i][0].lerp);// - rayInfos[i][0].distance;
            source.ViewDistance1 = Mathf.LerpUnclamped(ViewDistance0, ViewDistance1, rayInfos[i][last].lerp);// - rayInfos[i][last].distance;
            source.Reflection = Reflection;
            source.Layers = Layers;
            source.transform.parent = transform;
            source.Depth = Depth - 1;
            cur.SetActive(true);
            reflections.Add(cur);
        }
    }

    private bool clockwise(Vector2 a, Vector2 b, Vector2 c) => (b.x - a.x) * (c.y * a.y) - (c.x - a.x) * (b.y - a.y) < 0;

    private Vector4 BaseLerp(float t) =>
        new Vector4(
            Mathf.LerpUnclamped(Vector0.x, Vector1.x, t),
            Mathf.LerpUnclamped(Vector0.y, Vector1.y, t),
            Mathf.LerpAngle(Angle0, Angle1, t),
            Mathf.LerpUnclamped(ViewDistance0, ViewDistance1, t)
        );

    private RayInfo Cast(Vector2 origin, float angle, float t)
    {
        Vector2 angleVector = angle.Angle();
        float viewDistance = Mathf.LerpUnclamped(ViewDistance0, ViewDistance1, t);
        Vector2 transVector = transform.TransformDirection(angleVector);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.TransformPoint(origin), transVector, viewDistance, Layers);
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
                lerp = t
            };
        }
        return new RayInfo(origin + angleVector * viewDistance, Vector2.up, angleVector, viewDistance, false);
    }
}
