using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

class Parallax : MonoBehaviour
{
    private GameObject[] Backgrounds;
    [SerializeField] public Vector2[] Scales;
    private int Count => Backgrounds.Length;

    private void Reset()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (Transform t in transform) gameObjects.Add(t.gameObject);
        Backgrounds = gameObjects.ToArray();
        Scales = new Vector2[Count];
    }

    private void Awake() => OnValidate();

    private void OnValidate()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (Transform t in transform) gameObjects.Add(t.gameObject);
        Backgrounds = gameObjects.ToArray();
        if (Scales.Length != Count)
        {
            Debug.LogWarning("Careful with that");
            Array.Resize(ref Scales, Count);
        }
    }

    private void Update()
    {
        for (int i = 0; i < Count; i++)
        {
            Vector2 v = new Vector2(transform.position.x * -Scales[i].x, transform.position.y * -Scales[i].y);
            Backgrounds[i].transform.position = v;
        }
    }
}