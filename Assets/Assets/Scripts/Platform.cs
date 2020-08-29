using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
class Platform : MonoBehaviour
{
    private BoxCollider2D player;
    private BoxCollider2D boxCol;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float py = player.bounds.min.y;
        float ty = boxCol.bounds.max.y;
        bool b = py < ty - 0.1f;
        if(boxCol.isTrigger != b) boxCol.isTrigger = b;
    }
}