using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemies : MonoBehaviour
{
    [SerializeField] List<Collider2D> enemies = new List<Collider2D>();
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask enemiesLM;

    void Update()
    {
       
    }
    private void FixedUpdate()
    {
        if(enemies.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
