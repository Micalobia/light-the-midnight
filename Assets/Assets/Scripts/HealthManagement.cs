using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManagement : MonoBehaviour
{

    [SerializeField] private Animator healthAnim;
    [SerializeField] private PlayerController player;
    void Awake()
    {
        healthAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        HealthCheck();
    }

    void HealthCheck()
    {
        healthAnim.SetFloat("Health", player.Health);
    }
}
