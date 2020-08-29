using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{

    [SerializeField] private PlayerController playerCont;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            playerCont.Health -= 1;
        }
    }
}
