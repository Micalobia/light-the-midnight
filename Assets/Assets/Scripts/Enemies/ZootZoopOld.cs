using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZootZoopOld : MonoBehaviour
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
    [SerializeField] private GameObject spawnTrigger;

    [SerializeField] private AudioClip zoopClip;
    [SerializeField] private AudioClip zoopDeath;
    private AudioSource zoopSource;

    private void Awake()
    {
        zoopSource = gameObject.GetComponent<AudioSource>();
        zootZoopRB = GetComponent<Rigidbody2D>();
        zootZoopAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
       


    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("StartDeath");

        if(player.transform.position.x > spawnTrigger.transform.position.x)
        {
            zootZoopAnim.SetBool("ableToSpawn", true);
        }

        if (circuitBreak.enabled && player.activeInHierarchy && hasSpawned)
        {
            zootZoopRB.transform.Translate(-Vector2.right * speed, 0f);
            destroyTimer++;
        }
        
        if(!circuitBreak.enabled)
        {
            zootZoopAnim.SetBool("stopped", true);
        }
    }

    private void FixedUpdate()
    {
       
      
    }

    IEnumerator StartDeath()
    {
        zoopSource.PlayOneShot(zoopDeath);
        yield return new WaitUntil(() => destroyTimer > maxDestroyTimer);
        destroyTimer = 0;
        zootZoopAnim.SetBool("hasSpawned", false);
        hasSpawned = false;
        this.transform.position = new Vector3 (zootZoopTM.position.x,zootZoopTM.position.y);
        
    }

    void Spawn()
    {
        zoopSource.PlayOneShot(zoopClip);
        zootZoopAnim.SetBool("hasSpawned", true);
        hasSpawned = true;

    }
}
