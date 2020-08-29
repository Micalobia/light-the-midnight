using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDamage : MonoBehaviour, IEnemy
{

    public float health => Health; 
    [SerializeField] private float Health = 1;
    [SerializeField] private Animator bossAnim;
    [SerializeField] private BossController bossController;
    [SerializeField] private AudioClip hurtclip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject poof;
    [SerializeField] private Animator poofAnim;

    void Awake()
    {
        bossAnim = GetComponentInParent<Animator>();
        bossController = GetComponentInParent<BossController>();
        poof.SetActive(false);
    }
    
   
    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if(health <= 0)
        {
            StartCoroutine("EyeDeath");
        }
    }

    IEnumerator EyeDeath()
    {
        bossAnim.SetBool("EyeIsActive", false);
        bossController.bossHealth -= 1;
        poof.SetActive(true);
        poofAnim.SetTrigger("poofActive");
        yield return new WaitForSeconds(poofAnim.GetCurrentAnimatorStateInfo(0).length + poofAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Destroy(this.gameObject);
        audioSource.PlayOneShot(hurtclip);
    }

}
