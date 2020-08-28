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
    [SerializeField] public GameObject damageField;
    [SerializeField] public float Timer;
    [Header("Audio sources")]
    [SerializeField] private AudioSource roakAudioSource;
    [SerializeField] private AudioClip agroAudio;
    [SerializeField] private AudioClip deathAudio;

    private Transform player;
    private bool soundPlayed;
    private Rigidbody2D roakRb;
    private Animator roakAnim;
    private bool IsFacingLeft;
    private bool hasSpawned;
    private float scaleX;
    private float scaleY;
    #endregion


    void Awake()
    {
        roakHitBox = GetComponent<BoxCollider2D>();
        roakAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        roakRb = GetComponent<Rigidbody2D>();
        roakAudioSource = GetComponent<AudioSource>();
        hasSpawned = false;
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
    }

    // Update is called once per frame
    void Update() => CheckDistance();



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


                if (IsFacingLeft && Timer < 5)
                {
                    transform.Translate(-Speed * Time.deltaTime, 0f, 0f);
                    transform.localScale = new Vector2(scaleX, scaleY);
                    Timer += Time.deltaTime;

                    if (Timer > 5)
                    {
                        IsFacingLeft = false;
                        Timer = 0;
                    }
                }
                else if (!IsFacingLeft && Timer < 5)
                {
                    transform.Translate(Speed * Time.deltaTime, 0f, 0f);
                    transform.localScale = new Vector2(-scaleX, scaleY);
                    Timer += Time.deltaTime;

                    if (Timer > 5)
                    {
                        IsFacingLeft = true;
                        Timer = 0;
                    }
                }
            }

        }
    }
    #endregion

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if (health <= 0)
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
        if (objEnv.gameObject.CompareTag("Obstacle") && IsFacingLeft == true)
        {

            IsFacingLeft = false;
            Timer = 0;

        }
        else if (objEnv.gameObject.CompareTag("Obstacle") && IsFacingLeft == false)
        {
            IsFacingLeft = true;
            Timer = 0;
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

    public void EnableHitBox()
    {
        damageField.SetActive(!damageField.activeInHierarchy);
        //if (damageField.activeInHierarchy == false)
        //{
        //    damageField.SetActive(true);
        //}
        //else if (damageField.activeInHierarchy == true)
        //{
        //    damageField.SetActive(false);
        //}
    }
}
