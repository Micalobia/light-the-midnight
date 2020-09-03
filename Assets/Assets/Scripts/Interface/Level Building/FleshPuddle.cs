using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class FleshPuddle : MonoBehaviour
{
    [SerializeField] private GameObject[] Enemies;

    [SerializeField] private AudioClip puddleClip;
    private AudioSource puddleSource;

    private void Awake()
    {
        puddleSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.CompareTag("Player"))
                puddleSource.PlayOneShot(puddleClip);
    }

    private void Update()
    {
        for (int i = 0; i < Enemies.Length; i++) if (!Enemies[i].IsDestroyed()) return;
        Destroy(gameObject);
    }
}
