
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator bossAnim;
    private int eyeNum;
    private int tentacleNum;
    [SerializeField] private ParticleSystem laser;
    [SerializeField] private EnableDamage[] eyes;
    [SerializeField] public float bossHealth;
    [SerializeField] private Animator cutSceneAnimator;
    
    void Awake()
    {
        bossAnim = GetComponent<Animator>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tentacleNum = Random.Range(0, 6);
        bossAnim.SetInteger("ActiveTentacle", tentacleNum);
        tentacleNum = 0;

        eyeNum = Random.Range(0, 4);
        bossAnim.SetInteger("ActiveEye", eyeNum);
        bossAnim.SetBool("EyeIsActive", true);
        eyeNum = 0;

        checkHealth();
    }

    void checkHealth()
    {
       if(bossHealth <= 0)
        {
            StartCoroutine("CutScene");
        }
    }

    IEnumerator CutScene()
    {
        cutSceneAnimator.SetBool("bossIsDead", true);
        yield return new WaitForSeconds(cutSceneAnimator.GetCurrentAnimatorStateInfo(0).length + cutSceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        SceneManager.LoadScene("DeadBossScene");
    }
}