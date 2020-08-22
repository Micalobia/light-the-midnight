using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform laserPoint;
    public GameObject impact;

    public GameObject Arm;
    public GameObject Player;

    private bool isFacingRight;



    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;


        if (mousePosition.x < 0 && !isFacingRight)
        {
            Flip();
        }

        if(mousePosition.x > 0 && isFacingRight)
        {
            Flip();
        }

        if (Input.GetButtonDown("Fire1"))
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

    void Flip()
    {
        isFacingRight = !isFacingRight;

        if(isFacingRight)
        {
            Arm.transform.localScale = new Vector3 (.54f, -.53f, 0f);
        }

        if (!isFacingRight)
        {
            Arm.transform.localScale = new Vector3(.54f, .53f, 0f);
        }
    }
}
