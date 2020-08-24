﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Roak : MonoBehaviour, IEnemy
{
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


    void Awake()
    {
        roakAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        roakRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();

       
    }



    public void CheckDistance()
    {
        if(Vector2.Distance(transform.position, player.transform.position) < agroDistance)
        {
            
            roakAnim.SetBool("isWalkingNearPlayer", true);
            
            if(player.transform.position.y <= 10)
            {

                if ((player.transform.position.x + transform.position.x) > 0)
                {
                    transform.localScale = new Vector3(-.5f, .5f);
                }
                if ((player.transform.position.x - transform.position.x) < 0)
                {
                    transform.localScale = new Vector2(.5f, .5f);

                }

                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
               
            }
           
        }
        else
        {
            roakAnim.SetBool("isWalkingNearPlayer", false);
            roakAnim.SetBool("isWalking", true);


            if (isFacingLeft)
            {
                transform.Translate(-speed * Time.deltaTime, 0f, 0f);
                transform.localScale = new Vector2(.5f, .5f);
            }
            else if(!isFacingLeft)
            {
                transform.Translate(speed * Time.deltaTime, 0f, 0f);
                transform.localScale = new Vector2(-.5f, .5f);
            }



        }
    }

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if(health <= 0)
        {
            roakAnim.SetBool("isDead", true);
            if(roakAnim.GetBool("isWalkingNearPlayer") != true)
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
          Debug.Log("Accessed Is Facing Left");
          
        }
        else if (objEnv.gameObject.CompareTag("Obstacle") && isFacingLeft == false)
        {
            isFacingLeft = true;
        }
       
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }
}
