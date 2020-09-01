using System;
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
    [SerializeField] private int index;

    void Awake()
    {
        bossAnim = GetComponentInParent<Animator>();
        bossController = GetComponentInParent<BossController>();
        poof.SetActive(false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon>().OnLightTrigger += OnLightTrigger;
    }

    private void OnLightTrigger(ref Collider2D collider)
    {
        if (health > 0 && collider.gameObject == gameObject && bossAnim.GetBool("EyeIsActive") && bossAnim.GetInteger("ActiveEye") == index)
        {
            //The if-statement here is supposed to check if the eye is open before damaging, but it just... doesn't?
            takeDamage(1f);
        }
    }

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if (health <= 0)
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
        Destroy(gameObject);
        audioSource.PlayOneShot(hurtclip);
    }

}
