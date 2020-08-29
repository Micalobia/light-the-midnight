using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float JumpForce;
    //[SerializeField] public float JumpHeight;
    [SerializeField] [Range(0, .3f)] public float MovementSmoothing;
    [Header("Combat")]
    [SerializeField] public float Health;
    [SerializeField] public float Damage;
    [SerializeField] public float Knockback;
    [SerializeField] [Range(0f, 90f)] public float ArmDeadzone = 10f;

    [SerializeField] private bool isInvincible;
    [SerializeField] private float pickupValue;
    [SerializeField] private float maxHealth;

    //Variable to help detemine when the sprite flips. 
    private float horizontalMove = 0f;
    private Rigidbody2D playerRB;
    private GameObject arm;
    private Animator playerAnim;
    private bool isOnGround;
    private Vector3 currentVelocity = Vector3.zero;
    private bool playerLeft;
    private bool armLeft;
    private float moveDeadzone = 1f;

    private void Reset()
    {
        MoveSpeed = 50f;
        JumpForce = 10f;
        Health = 3f;
        Damage = 1f;
        Knockback = 20;
        MovementSmoothing = .05f;
        ArmDeadzone = 10f;
    }

    void Awake()
    {
        maxHealth = Health;
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        arm = transform.GetChild(0).gameObject;
        FlipArm();
    }
    void Update()
    {

        if(Health > 0)
        {
            //Sets the Horizontal Move Variable for the Movement method

            horizontalMove = Input.GetAxis("Horizontal") * MoveSpeed;

            ArmToMouse();

             //Makes the player jump
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isOnGround != false)
            {
            playerRB.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            isOnGround = false;
            playerAnim.SetBool("hasJumped", true);
            }
        }
       

    }

    void FixedUpdate() => Move(horizontalMove * Time.fixedDeltaTime);

    public void OnTriggerEnter2D(Collider2D hitBox)
    {
        if (hitBox.CompareTag("HurtBox"))
        {
            if (!isInvincible && Health > 0)
            {
                Health -= Damage;

                if (playerLeft)
                {
                    playerRB.AddForce(-Vector2.right * Knockback, ForceMode2D.Impulse);
                }

                if (!playerLeft)
                {
                    playerRB.AddForce(Vector2.right * Knockback, ForceMode2D.Impulse);
                }

                CheckHealth();
            }
        }

        if (hitBox.CompareTag("HealthPickup") && Health < maxHealth)
        {
            Health += pickupValue;
            Destroy(hitBox.gameObject);

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;

        }

        if (other.gameObject.CompareTag("HurtBox"))
        {
            if (!isInvincible && Health > 0)
            {
                Health -= Damage;

                if (playerLeft)
                {
                    playerRB.AddForce(-Vector2.right * Knockback, ForceMode2D.Impulse);
                }

                if (!playerLeft)
                {
                    playerRB.AddForce(Vector2.right * Knockback, ForceMode2D.Impulse);
                }

                CheckHealth();
            }
        }
    }

    #region Movement

    /// <summary>
    /// https://www.youtube.com/watch?v=dwcT-Dch0bA&t=177s
    ///  *Based on code from Brackeys on YouTube 
    /// </summary>

    //Causes character to move. 
    void Move(float move)
    {
        if(Health > 0)
        {
            Vector2 mousePosition = Input.mousePosition;

            Vector3 targetVelocity = new Vector2(move * 10f, playerRB.velocity.y);
            playerRB.velocity = Vector3.SmoothDamp(playerRB.velocity, targetVelocity, ref currentVelocity, MovementSmoothing);

            float angle = arm.transform.up.Angle();
            float deadsup = 180f - ArmDeadzone;
            if (armLeft && angle < -ArmDeadzone && angle > -deadsup)
            {
                FlipArm();
            }
            if (!armLeft && angle > ArmDeadzone && angle < deadsup)
            {
                FlipArm();
            }
            if (!playerLeft && playerRB.velocity.x < -moveDeadzone) FlipPlayer();
            if (playerLeft && playerRB.velocity.x > moveDeadzone) FlipPlayer();

            //if ((move * Time.deltaTime < 0 && !isFacingLeft) || (mousePosition.x < -90 && !isFacingLeft && move == 0)) Flip();

            //if ((move * Time.deltaTime > 0 && isFacingLeft) || (mousePosition.x > -90 && isFacingLeft && move == 0))
            //{
            //    Flip();
            //}

            //This is what triggers the animations for the player.
            if (move != 0 && Health > 0)
            {
                playerAnim.SetBool("isRunning", true);
            }
            else if (move == 0 && Health > 0)
            {
                playerAnim.SetBool("isRunning", false);
            }
        }
    }

    //Method to make the arm look at the mouse. 
    void ArmToMouse()
    {
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 point = new Vector2(

            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y

            );

        arm.transform.right = point;
    }

    //Method that flips the game object when it is moving 
    void FlipPlayer()
    {
        playerLeft = !playerLeft;

        //Vector3 v = transform.localScale;
        //v.x *= -1;
        //transform.localScale = v;
        transform.Rotate(0f, 180f, 0f);
    }

    void setInvincible()
    {
        isInvincible = true;
    }

    void setVulnerable()
    {
        isInvincible = false;
    }

    void FlipArm()
    {
        armLeft = !armLeft;

        Vector3 v = arm.transform.localScale;
        v.y *= -1;
        arm.transform.localScale = v;
    }

    void CheckHealth()
    {
        if (Health > 0)
        {
            playerAnim.SetTrigger("tookDamage");
        }

        if (Health <= 0)
        {
            playerAnim.SetBool("isDead", true);
            

        }
    }

    void isFalling()
    {
        if (isOnGround == true)
        {
            playerAnim.SetBool("hasJumped", false);
            playerAnim.SetBool("isFalling", false);
        }
        else
        {
            playerAnim.SetBool("hasJumped", false);
            playerAnim.SetBool("isFalling", true);
        }
    }
    #endregion Movement
}
