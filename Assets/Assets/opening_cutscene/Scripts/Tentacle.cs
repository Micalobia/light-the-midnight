using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private BossController Boss;

    void Awake()
    {
        Boss = GetComponentInParent<BossController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
