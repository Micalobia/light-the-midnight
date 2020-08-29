using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Roak : MonoBehaviour, IEnemy
{
    #region Variables
    public float health => Health;
    [SerializeField] private float SpriteOffset;
    [SerializeField] protected float Health = 100;
    public BoxCollider2D roakHitBox;
    [SerializeField] public float Speed;
    [SerializeField] public float AgroDistance;
    [SerializeField] public float AttackDistance;
    [SerializeField] public bool Started;
    [SerializeField] public EnemyTrigger SpawnTrigger;
    [Header("Audio sources")]
    [SerializeField] private AudioClip agroAudio;
    [SerializeField] private AudioClip deathAudio;

    private float timer;
    private AudioSource roakAudioSource;
    private Transform player;
    private GameObject damageField;
    private bool soundPlayed;
    private Rigidbody2D roakRb;
    private Animator roakAnim;
    private bool isFacingLeft;
    private bool hasSpawned;
    private float scaleX;
    private float scaleY;
    #endregion

    void Start()
    {
        roakHitBox = GetComponent<BoxCollider2D>();
        roakAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        roakRb = GetComponent<Rigidbody2D>();
        roakAudioSource = GetComponent<AudioSource>();
        damageField = transform.GetChild(0).gameObject;
        hasSpawned = false;
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        gameObject.SetActive(Started);
        roakAnim.SetBool("Started", Started);
        SpawnTrigger.OnEnemyTrigger += () =>
        {
            Started = true;
            gameObject.SetActive(Started);
            roakAnim.SetBool("Started", Started);
        };
        FindObjectOfType<LightSourceHolder>().OnLightTrigger += OnLightTrigger;
        //player.GetComponentInChildren<LightSourcePoint>().OnLightTrigger += OnLightTrigger;
    }

    // Update is called once per frame
    void Update() => CheckDistance();

    private void OnLightTrigger(ref Collider2D col)
    {
        if (col.GetInstanceID() == roakHitBox.GetInstanceID())
        {
            takeDamage(Health);
            Debug.Log("Ouchie");
        }

    }

    #region Movement
    public void CheckDistance()
    {
        if (hasSpawned == true)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < AgroDistance)
            {
                if (!soundPlayed)
                {
                    roakAudioSource.PlayOneShot(agroAudio);
                    soundPlayed = true;
                }
                roakAnim.SetBool("isWalkingNearPlayer", true);
                if (player.transform.position.y <= 10)
                {
                    if ((player.transform.position.x + transform.position.x) > 0)
                    {
                        transform.localScale = new Vector3(-scaleX, scaleY);
                        Attack();
                    }
                    if ((player.transform.position.x - transform.position.x) < 0)
                    {
                        transform.localScale = new Vector2(scaleX, scaleY);
                        Attack();
                    }
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), Speed * Time.deltaTime);
                }

            }
            else
            {
                roakAnim.SetBool("isWalkingNearPlayer", false);
                roakAnim.SetBool("isWalking", true);


                if (isFacingLeft && timer < 5)
                {
                    transform.Translate(-Speed * Time.deltaTime, 0f, 0f);
                    transform.localScale = new Vector2(scaleX, scaleY);
                    timer += Time.deltaTime;

                    if (timer > 5)
                    {
                        isFacingLeft = false;
                        timer = 0;
                    }
                }
                else if (!isFacingLeft && timer < 5)
                {
                    transform.Translate(Speed * Time.deltaTime, 0f, 0f);
                    transform.localScale = new Vector2(-scaleX, scaleY);
                    timer += Time.deltaTime;

                    if (timer > 5)
                    {
                        isFacingLeft = true;
                        timer = 0;
                    }
                }
            }

        }
    }
    #endregion

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            roakAudioSource.PlayOneShot(deathAudio);



            roakAnim.SetBool("isDead", true);


            if (roakAnim.GetBool("isWalkingNearPlayer") != true)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y);

            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - SpriteOffset);
            }
            roakHitBox.enabled = false;

        }
    }

    public void OnCollisionEnter2D(Collision2D objEnv)
    {
        if (objEnv.gameObject.CompareTag("Obstacle") && isFacingLeft == true)
        {

            isFacingLeft = false;
            timer = 0;

        }
        else if (objEnv.gameObject.CompareTag("Obstacle") && isFacingLeft == false)
        {
            isFacingLeft = true;
            timer = 0;
        }

    }

    public void Death() => Destroy(gameObject);

    public void Attack()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < AttackDistance)
        {
            roakAnim.SetTrigger("Attack");
        }
    }

    public void Spawn()
    {
        roakAnim.SetTrigger("hasSpawned");
        hasSpawned = true;
    }

    public void EnableHitBox() => damageField.SetActive(!damageField.activeInHierarchy);//if (damageField.activeInHierarchy == false)//{//    damageField.SetActive(true);//}//else if (damageField.activeInHierarchy == true)//{//    damageField.SetActive(false);//}
}
