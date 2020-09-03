using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
class ZootZoop : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float PatrolTime;
    [SerializeField] private float PatrolSpeed;
    [SerializeField] [Range(0f, 1f)] private float PatrolOffset;
    [SerializeField] private bool MovingRight;
    [Header("Audio")]
    [SerializeField] private float ZoomDistance;
    [SerializeField] private AudioClip ZoomClip;
    [SerializeField] private AudioClip OnDeathClip;

    private Vector2 Center => transform.position;
    private Vector2 PlayerCenter => _player.position;

    private Animator _anim;
    private Vector2 _patrolRoot;
    private Vector2 _patrolLeft;
    private Vector2 _patrolRight;
    private bool _dead;
    private float _patrolDistance;
    private bool _facingRight;
    private BoxCollider2D _mainCol;
    private CircleCollider2D _painCol;
    private AudioSource _source;
    private Vector2 _velocity;
    private bool _zoom;
    private Transform _player;

    private void OnValidate()
    {
        _patrolDistance = PatrolTime * PatrolSpeed;
        _patrolLeft = _patrolRoot - new Vector2((1f - PatrolOffset) * _patrolDistance, 0);
        _patrolRight = _patrolRoot + new Vector2(_patrolDistance * PatrolOffset, 0);
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _patrolRoot = Center;
        OnValidate();
        _mainCol = GetComponent<BoxCollider2D>();
        _painCol = GetComponent<CircleCollider2D>();
        _source = GetComponent<AudioSource>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        LightSourceHolder[] holders = FindObjectsOfType<LightSourceHolder>();
        foreach (var holder in holders)
        {
            if (holder.transform.parent != null && holder.transform.parent.TryGetComponent(out LightSourceHolder _)) continue;
            holder.OnLightTrigger += OnLightTrigger;
        }
    }

    private void Update()
    {
        if (_dead) return;
        Patrol();
        UpdatePosition();
        UpdateDirection();
        bool b = Vector2.Distance(Center, PlayerCenter) < ZoomDistance;
        if (!_zoom && b) ZoomNoise();
        _zoom = b;
    }

    private void UpdatePosition() => transform.position += (Vector3)_velocity;
    private void UpdateDirection() { if (_facingRight != _velocity.x > 0) Flip(); }
    private void Flip() => transform.rotation = Quaternion.AngleAxis((_facingRight = _velocity.x > 0f) ? 180f : 0f, Vector3.up);

    private void Patrol()
    {
        Vector2 point = MovingRight ? _patrolRight : _patrolLeft;
        float diff = point.x - Center.x;
        MovingRight ^= Pursue(diff, PatrolSpeed);
    }

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

    private void StartDeath()
    {
        if (!_dead)
        {
            _dead = true;
            _anim.SetTrigger("Dead");
            _mainCol.enabled = false;
            _painCol.enabled = false;
        }
    }

    private void OnLightTrigger(ref Collider2D collider) { if (collider == _painCol) StartDeath(); }
    private void ZoomNoise() => _source.PlayOneShot(ZoomClip);
    private void DeathNoise() => _source.PlayOneShot(OnDeathClip);
    private void Die() => Destroy(gameObject);
}

