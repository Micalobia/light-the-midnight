using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
class Battery : MonoBehaviour
{
    private Animator anim;
    private Weapon player;
    private int charges;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Weapon>();
    }

    private void Update()
    {
        Debug.Log(player.ChargesLeft);
        if (player.ChargesLeft != charges)
        {
            charges = player.ChargesLeft;
            anim.SetInteger("ChargesLeft", charges);
        }
    }
}

