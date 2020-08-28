using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator bossAnim;
    private int tentacleNum;

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
    }

    IEnumerator TentacleSelect()
    {
       

        yield return new WaitForSecondsRealtime(3);
       
    }
}
