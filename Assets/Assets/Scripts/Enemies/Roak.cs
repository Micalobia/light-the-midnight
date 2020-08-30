using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
class Roak : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float PatrolTime;
    [SerializeField] private float PatrolSpeed;
    [SerializeField] [Range(0f, 1f)] private float PatrolOffset;
    [SerializeField] private bool MovingRight;
    [Header("Combat")]
    [SerializeField] private float AgroRange;
    [SerializeField] private float AgroSpeed;
    [Header("Audio")]
    [SerializeField] private AudioClip OnAgroClip;
    [SerializeField] private AudioClip OnDeathClip;
    [Header("Other")]
    [SerializeField] private EnemyTrigger SpawnTrigger;
    [SerializeField] private bool UseTrigger;

    private Vector2 PlayerCenter => _playerTransform.position;
    private Vector2 Center => transform.position;

    private GameObject _player;
    private PlayerController _playerController;
    private Transform _playerTransform;
    private Animator _anim;
    private Vector2 _center;
    private Vector2 _playerCenter;
    private float _sqrAgro;
    private Vector2 _patrolRoot;
    private Vector2 _patrolLeft;
    private Vector2 _patrolRight;
    private bool _dead;
    private float _patrolDistance;
    private Vector2 _mag;
    private bool _facingRight;
    private Vector2 _velocity;
    private bool _ready;
    private CapsuleCollider2D _mainCol;
    private AudioSource _source;
    private bool _ableToAttack;
    private bool _agro;
    private bool _spawned;

    private void OnValidate()
    {
        _sqrAgro = AgroRange * AgroRange;
        _patrolDistance = PatrolTime * PatrolSpeed;
        _patrolLeft = _patrolRoot - new Vector2((1f - PatrolOffset) * _patrolDistance, 0);
        _patrolRight = _patrolRoot + new Vector2(_patrolDistance * PatrolOffset, 0);
    }

    private void Reset()
    {
        PatrolSpeed = 4f;
        PatrolTime = 2f;
        PatrolOffset = 0f;
        MovingRight = true;
        AgroRange = 10f;
        AgroSpeed = 5f;
        OnAgroClip = null;
        OnDeathClip = null;
        UseTrigger = false;
        SpawnTrigger = null;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _playerTransform = _player.GetComponent<Transform>();
        _player.GetComponent<Weapon>().OnLightTrigger += OnLightTrigger;
        _anim = GetComponent<Animator>();
        _patrolRoot = Center;
        OnValidate();
        _ready = !UseTrigger;
        _anim.SetBool("Ready", _ready);
        gameObject.SetActive(_ready);
        if (UseTrigger) SpawnTrigger.OnEnemyTrigger += () =>
        {
            gameObject.SetActive(true);
            _anim.SetBool("Ready", _ready = true);
        };
        LightSourceHolder holder = FindObjectOfType<LightSourceHolder>();
        if (holder != null) holder.OnLightTrigger += OnLightTrigger;
        _mainCol = GetComponent<CapsuleCollider2D>();
        _source = GetComponent<AudioSource>();
        _ableToAttack = true;
        _spawned = false;
    }

    private void Update()
    {
        if (_dead || _spawned) return;
        _center = Center;
        _playerCenter = PlayerCenter;
        _mag = _playerCenter - _center;
        float sqr = _mag.sqrMagnitude;
        if (sqr < _sqrAgro) Agro();
        else Patrol();
        UpdatePosition();
        UpdateDirection();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_ableToAttack && collision.gameObject == _player)
        {
            _playerController.DamagePlayer(1);
            _anim.SetTrigger("Attacking");
            _ableToAttack = false;
        }
    }
    private void DoneAttacking() => _ableToAttack = true;

    private void UpdatePosition() => transform.position += (Vector3)_velocity;
    private void UpdateDirection() { if (_facingRight != _velocity.x > 0) Flip(); }
    private void Flip() => transform.rotation = Quaternion.AngleAxis((_facingRight = _velocity.x > 0f) ? 180f : 0f, Vector3.up);
    private void OnLightTrigger(ref Collider2D collider) { if (collider == _mainCol) StartDeath(); }
    private bool Pursue(float diff, float Speed)
    {
        float sign = Mathf.Sign(diff);
        float mag = Mathf.Abs(diff);
        float speed = Speed * Time.deltaTime;
        bool at = mag < speed;
        Vector2 vel = _velocity;
        vel.x = sign * (at ? mag : speed);
        _velocity = vel;
        return at;
    }
    private void SetAgro(bool value)
    {
        if (value && !_agro)
            _source.PlayOneShot(OnAgroClip);
        _agro = value;
        _anim.SetBool("Agro", value);
    }
    private void Agro()
    {
        if(!_player.activeInHierarchy)
        {
            Patrol();
            return;
        }
        SetAgro(true);
        float diff = _mag.x;
        Pursue(_mag.x, AgroSpeed);
    }
    private void Patrol()
    {
        _anim.SetBool("Agro", false);
        Vector2 point = MovingRight ? _patrolRight : _patrolLeft;
        float diff = point.x - _center.x;
        MovingRight ^= Pursue(diff, PatrolSpeed);
    }
    private void StartDeath()
    {
        if (!_dead) _source.PlayOneShot(OnDeathClip);
        _dead = true;
        _anim.SetBool("Dead", true);
    }
    private void Die() => Destroy(gameObject);
    private void Spawned() => _spawned = true;
}


