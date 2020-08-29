using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDamage : MonoBehaviour, IEnemy
{

    public float health => Health; 
    [SerializeField] private float Health = 1;
    [SerializeField] private Animator bossAnim;
    [SerializeField] private BossController bossController;

    void Awake()
    {
        bossAnim = GetComponentInParent<Animator>();
        bossController = GetComponentInParent<BossController>();
    }
    
   
    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if(health <= 0)
        {
            bossAnim.SetBool("EyeIsActive", false);
            bossController.bossHealth -= 1;
            Destroy(this.gameObject);
        }
    }


}
