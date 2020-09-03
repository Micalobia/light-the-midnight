using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EntranceTrigger : MonoBehaviour
{
    [SerializeField] private GameObject dialogueTrigger;
    [SerializeField] private GameObject[] Enemies;

    private void Update()
    {
        for (int i = 0; i < Enemies.Length; i++) if (!Enemies[i].IsDestroyed()) return;
        dialogueTrigger.SetActive(true);
    }
}
