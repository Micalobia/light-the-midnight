using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform laserPoint;
    public GameObject impact; 

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit2D projectLaser = Physics2D.Raycast(laserPoint.position, laserPoint.right);

        if (projectLaser)
        {
            Instantiate(impact, projectLaser.point, Quaternion.identity);
        }
    }
}
