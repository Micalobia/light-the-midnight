using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Roak : MonoBehaviour, IEnemy
{
    #region Variables
    public virtual float health => Health;
    [SerializeField]
    private float spriteOffset;
    [SerializeField]
    protected float Health = 100;
    [SerializeField]
    private Animator roakAnim;
    [SerializeField]
    private BoxCollider2D roakHitBox;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float agroDistance;
    private Rigidbody2D roakRb;
    [SerializeField]
    private bool isFacingLeft;
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    public GameObject damageField;
    [SerializeField]
    private bool hasSpawned;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private float Scale;
    [SerializeField]
    private float Timer;

    //Sound Files for Roak Damage
    [SerializeField]
    private AudioSource roakAudioSource;

    [SerializeField]
    private AudioClip agroAudio;
    [SerializeField]
    private AudioClip deathAudio;

    private bool soundPlayed;

    #endregion


    void Awake()
    {
        roakAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        roakRb = GetComponent<Rigidbody2D>();
        roakAudioSource = GetComponent<AudioSource>();
        hasSpawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();

       
    }

   

    #region Movement
    public void CheckDistance()
    {
        if (hasSpawned == true)
        {
            

            if (Vector2.Distance(transform.position, player.transform.position) < agroDistance)
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
                        transform.localScale = new Vector3(-.5f, .5f);
                        Attack();
                    }
                    if ((player.transform.position.x - transform.position.x) < 0)
                    {
                        transform.localScale = new Vector2(.5f, .5f);
                        Attack();

                    }

                    transform.position = Vector2.MoveTowards(transform.position, new Vector2 (player.transform.position.x, transform.position.y), speed * Time.deltaTime);

                }

            }
            else
            {
                roakAnim.SetBool("isWalkingNearPlayer", false);
                roakAnim.SetBool("isWalking", true);


                if (isFacingLeft && Timer < 5)
                {
                    transform.Translate(-speed * Time.deltaTime, 0f, 0f);
                    transform.localScale = new Vector2(Scale, Scale);
                    Timer += Time.deltaTime;

                    if(Timer > 5)
                    {
                        isFacingLeft = false;
                        Timer = 0;
                    }
                }
                else if (!isFacingLeft && Timer < 5)
                {
                    transform.Translate(speed * Time.deltaTime, 0f, 0f);
                    transform.localScale = new Vector2(-Scale, Scale);
                    Timer += Time.deltaTime;

                    if(Timer > 5)
                    {
                        isFacingLeft = true;
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

        if(health <= 0)
        {
            roakAudioSource.PlayOneShot(deathAudio);



            roakAnim.SetBool("isDead", true);


            if (roakAnim.GetBool("isWalkingNearPlayer") != true)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y);

            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - spriteOffset);
            }
            roakHitBox.enabled = false;
            
        }
    }

    public void OnCollisionEnter2D(Collision2D objEnv)
    {
        if (objEnv.gameObject.CompareTag("Obstacle") && isFacingLeft == true)
        {
           
          isFacingLeft = false;
            Timer = 0;
          
        }
        else if (objEnv.gameObject.CompareTag("Obstacle") && isFacingLeft == false)
        {
            isFacingLeft = true;
            Timer = 0;
        }
       
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

    public void Attack()
    {
        if(Vector2.Distance(transform.position, player.transform.position) < attackDistance)
        {
            roakAnim.SetTrigger("Attack");
        }
    }

    public void spawn()
    {
        roakAnim.SetTrigger("hasSpawned");
        hasSpawned = true;
     
    }

    public void enableHitbox()
    {
        if(damageField.activeInHierarchy == false)
        {
            damageField.SetActive(true);
        }
        else if(damageField.activeInHierarchy == true)
        {
            damageField.SetActive(false);
        }
    }
}
