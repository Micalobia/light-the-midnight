using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] public GameObject Arm;
    [SerializeField] public GameObject Player;

    [SerializeField] public float RechargeTime;
    [SerializeField] public float OnTime;
    [SerializeField] LightSourcePoint source;
    [SerializeField] public GameObject CanvasMenu;

    public int ChargesLeft => charges;

    private const int _maxCharges = 4;
    private int charges;
    private bool isFacingRight;
    private float timeFired;

    private void Start() => charges = _maxCharges;

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (mousePosition.x > 0 && !isFacingRight)
        {
            Flip();
        }

        if (mousePosition.x < 0 && isFacingRight)
        {
            Flip();
        }

        if (charges == _maxCharges) timeFired = Time.time;
        if (CanvasMenu.activeInHierarchy) timeFired += Time.deltaTime;
        if(Time.time - timeFired > RechargeTime)
        {
            ++charges;
            timeFired = Time.time;
        }

        if (!CanvasMenu.activeInHierarchy && Input.GetButtonDown("Fire1") && charges != 0)
        {
            timeFired = Time.time;
            StopCoroutine("Shoot");
            StartCoroutine("Shoot");
        }
    }

    IEnumerator Shoot()
    {
        //RaycastHit2D projectLaser = Physics2D.Raycast(laserPoint.position, laserPoint.right);

        //if (projectLaser)
        //{
        //    Roak roak = projectLaser.transform.GetComponent<Roak>();

        //    if(roak != null)
        //    {
        //        roak.takeDamage(damage);
        //    }

        //    Instantiate(impact, projectLaser.point, Quaternion.identity);

        //    line.SetPosition(0, laserPoint.position);
        //    line.SetPosition(1, projectLaser.point);
        //}
        //else
        //{
        //    line.SetPosition(0, laserPoint.position);
        //    line.SetPosition(1, laserPoint.position + laserPoint.right * 100);
        //}

        //line.enabled = true;
        --charges;
        source.TurnedOn = true;
        yield return new WaitForSeconds(OnTime);
        source.TurnedOn = false;

        //line.enabled = false;

    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight)
        {
            Arm.transform.localScale = new Vector3(.54f, -.53f, 0f);
        }

        if (!isFacingRight)
        {
            Arm.transform.localScale = new Vector3(.54f, .53f, 0f);
        }
    }
}
