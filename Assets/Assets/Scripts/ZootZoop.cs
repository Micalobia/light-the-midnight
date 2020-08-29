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
    [SerializeField] public float destroyTimer;
    [SerializeField] public float maxDestroyTimer;
    [SerializeField] private bool hasSpawned;
    [SerializeField] private Transform zootZoopTM;

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

        if (circuitBreak.enabled && player.activeInHierarchy && hasSpawned)
        {
            zootZoopRB.transform.Translate(-Vector2.right * speed, 0f);
            destroyTimer++;
        }
    }

    private void FixedUpdate()
    {
       
      
    }

    IEnumerator StartDeath()
    {
        yield return new WaitUntil(() => destroyTimer > maxDestroyTimer);
        destroyTimer = 0;
        zootZoopAnim.SetBool("hasSpawned", false);
        hasSpawned = false;
        this.transform.position = new Vector3 (zootZoopTM.position.x,zootZoopTM.position.y);
        
    }

    void Spawn()
    {
        zootZoopAnim.SetBool("hasSpawned", true);
        hasSpawned = true;

    }
}
