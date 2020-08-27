using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float FlipDeadzone;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private Rigidbody2D playerRb;
    [SerializeField]
    private bool isOnGround;
    [SerializeField]
    private float health;
    [SerializeField]
    private GameObject arm;
    [SerializeField]
    private Animator playerAnim;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float knockBack;
    //Variable to help detemine when the sprite flips. 
    private float horizontalMove = 0f;



    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    private bool isFacingLeft;
    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
    }
    void Update()
    {
        //Sets the Horizontal Move Variable for the Movement method
        horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;

        ArmToMouse();

        //Makes the player jump
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isOnGround != false)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
            playerAnim.SetBool("hasJumped", true);
        }

    }

    void FixedUpdate() => Move(horizontalMove * Time.fixedDeltaTime);

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;

        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            health -= damage;

            if (isFacingLeft)
            {
                playerRb.AddForce(-Vector2.right * knockBack, ForceMode2D.Impulse);
            }

            if (!isFacingLeft)
            {
                playerRb.AddForce(Vector2.right * knockBack, ForceMode2D.Impulse);
            }

            CheckHealth();
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
        Vector2 mousePosition = Input.mousePosition;

        Vector3 targetVelocity = new Vector2(move * 10f, playerRb.velocity.y);
        playerRb.velocity = Vector3.SmoothDamp(playerRb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (playerRb.velocity.x + FlipDeadzone < 0 && !isFacingLeft) Flip();
        if (playerRb.velocity.x - FlipDeadzone > 0 && isFacingLeft) Flip();

        //if ((move * Time.deltaTime < 0 && !isFacingLeft) || (mousePosition.x < -90 && !isFacingLeft && move == 0)) Flip();

        //if ((move * Time.deltaTime > 0 && isFacingLeft) || (mousePosition.x > -90 && isFacingLeft && move == 0))
        //{
        //    Flip();
        //}

        //This is what triggers the animations for the player.
        if (move != 0 && health > 0)
        {
            playerAnim.SetBool("isRunning", true);
        }
        else if (move == 0 && health > 0)
        {
            playerAnim.SetBool("isRunning", false);
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
    void Flip()
    {
        isFacingLeft = !isFacingLeft;

        transform.Rotate(0f, 180f, 0f);
    }

    void CheckHealth()
    {
        if (health > 0)
        {
            playerAnim.SetTrigger("tookDamage");
        }

        if (health <= 0)
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
