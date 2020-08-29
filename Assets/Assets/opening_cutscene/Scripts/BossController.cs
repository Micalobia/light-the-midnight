using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator bossAnim;
    private int eyeNum;
    private int tentacleNum;
    [SerializeField] private ParticleSystem laser;
    [SerializeField] private EnableDamage[] eyes;
    [SerializeField] public float bossHealth;
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
        tentacleNum = Random.Range(0, 5);
        bossAnim.SetInteger("ActiveTentacle", tentacleNum);
        tentacleNum = 0;

        eyeNum = Random.Range(0, 4);
        bossAnim.SetInteger("ActiveEye", eyeNum);
        bossAnim.SetBool("EyeIsActive", true);
        eyeNum = 0;

        checkHealth();
    }

    void systemEnable()
    {
        laser.Play();
    }

    void systemStop()
    {
        laser.Stop();
    }

    void checkHealth()
    {
       if(bossHealth <= 0)
        {
            SceneManager.LoadScene("DeadBossScene");
        }
    }

}
