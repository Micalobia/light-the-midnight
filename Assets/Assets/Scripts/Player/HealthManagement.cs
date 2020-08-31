using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManagement : MonoBehaviour
{

    private Animator _healthAnim;
    private PlayerController _player;
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _healthAnim = GetComponent<Animator>();
    }

    private void Update() => HealthCheck();

    void HealthCheck() => _healthAnim.SetInteger("Health", _player.Health);
}
