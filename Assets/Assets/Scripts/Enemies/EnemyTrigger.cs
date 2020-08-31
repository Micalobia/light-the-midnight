using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyTrigger : MonoBehaviour
{
    public event OnEnemyTriggerDelegate OnEnemyTrigger;

    private void Awake() => GetComponent<BoxCollider2D>().isTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) OnEnemyTrigger?.Invoke();
    }
}

public delegate void OnEnemyTriggerDelegate();