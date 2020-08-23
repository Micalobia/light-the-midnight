using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    //Variable to help detemine when the sprite flips. 
    private float horizontalMove = 0f;

    
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f; 
    private Vector3 m_Velocity = Vector3.zero;

    bool isFacingRight;
    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
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
        }

    }

    void FixedUpdate()
    {
        Move(horizontalMove * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
           
        }

        if (other.gameObject.CompareTag("Enemy"))
        {

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

        if (move < 0 && !isFacingRight || mousePosition.x < -90 && !isFacingRight && move == 0)
        {
            for (int count = 0; count < 1; count++)
            {
                Flip();
                count++;
            }
            Debug.Log(mousePosition.x);
        }
  
        else if (move > 0 && isFacingRight || mousePosition.x > -90 && isFacingRight && move == 0)
        {

            for (int count = 0; count < 1; count++)
            {
                Flip();
                count++;
            }
            Debug.Log(mousePosition.x);
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
        isFacingRight = !isFacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    #endregion Movement
}
