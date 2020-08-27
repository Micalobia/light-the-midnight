using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoakReference : MonoBehaviour
{
    // Start is called before the first frame update

    public Roak roak;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Attack()
    {
        roak.Attack();
    }

    void spawn()
    {
        roak.spawn();
    }

    void enableHitbox()
    {
        roak.enableHitbox();
    }
}
