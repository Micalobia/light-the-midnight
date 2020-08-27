using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flockaroo : MonoBehaviour, IEnemy
{

    public virtual float health => Health;
    [SerializeField] protected float Health = 100;
    [SerializeField] private float moveSpeed;
    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if (health <= 0)
        {
          

        }
    }
}
