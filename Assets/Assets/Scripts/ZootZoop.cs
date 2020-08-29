using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZootZoop : MonoBehaviour
{
    [SerializeField] private Rigidbody2D zootZoopRB;
    [SerializeField] private Animator zootZoopAnim;
    [SerializeField] private float speed;
    [SerializeField] private BoxCollider2D circuitBreak;
    [SerializeField] private GameObject player;
    [SerializeField] private float destroyTimer;
    [SerializeField] private float maxDestroyTimer;
    [SerializeField] private bool hasSpawned;
    private void Awake()
    {
        zootZoopRB = GetComponent<Rigidbody2D>();
        zootZoopAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("StartDeath");
    }

    private void FixedUpdate()
    {
        if (circuitBreak.enabled && player.activeInHierarchy && hasSpawned)
        {
           
            zootZoopRB.AddForce(-Vector2.right * speed, ForceMode2D.Impulse);
            destroyTimer++;
        }
      
    }

    IEnumerator StartDeath()
    {
        yield return new WaitUntil(() => destroyTimer > maxDestroyTimer);
        Destroy(this.gameObject);
    }

    void Spawn()
    {
        zootZoopAnim.SetBool("hasSpawned", true);
        hasSpawned = true;

    }
}
