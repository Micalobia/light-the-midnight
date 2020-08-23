using System.Collections;
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

    void Awake()
    {
        roakAnim = GetComponent<Animator>();
        roakHitBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if(health <= 0)
        {
            roakAnim.SetBool("isDead", true);
            transform.position = new Vector2(transform.position.x, transform.position.y - spriteOffset);
            roakHitBox.enabled = false;
        }
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }
}
