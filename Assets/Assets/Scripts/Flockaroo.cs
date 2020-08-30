using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Flockaroo : MonoBehaviour, IEnemy
{
    public virtual float health => Health;
    [SerializeField] protected float Health = 100f;
    [Header("Patrol")]
    [SerializeField] private Vector2 PatrolPoint1;
    [SerializeField] private Vector2 PatrolPoint2;
    [SerializeField] private float PatrolSpeed;
    [Header("Shiny")]
    [SerializeField] private float ShinySpeed;
    [SerializeField] private float ShinyRange;
    [Header("Combat")]
    [SerializeField] private float DiveRange;
    [SerializeField] private Vector2 DiveScale;
    [SerializeField] private float DiveAngle;
    [SerializeField] private float DiveTime;
    [SerializeField] private float DiveCooldown;
    [SerializeField] private float AgroRange;
    [SerializeField] private float AgroSpeed;

    private Vector2 WorldCenter => transform.position;
    private Vector2 PlayerCenter => _player.position;

    private LightSourceHolder _lights;
    private bool _movingTo2;
    private Animator _anim;
    private bool _dead;
    private bool _right;
    private Vector2 _center;
    private SpriteRenderer _rend;
    private BoxCollider2D _boxCol;
    private Vector2 _velocity;
    private bool _diving;
    private Vector2 freezeDive;
    private float _diveCooldown;
    private Transform _player;

    private void Reset()
    {
        PatrolPoint1 = new Vector2();
        PatrolPoint2 = new Vector2();
        PatrolSpeed = 4f;
        ShinyRange = 5f;
        Health = 100f;
        ShinySpeed = 8f;
        ShinyRange = 12f;
        DiveRange = 8f;
        AgroRange = 16f;
        AgroSpeed = 6f;
    }

    private void Start()
    {
        _lights = FindObjectOfType<LightSourceHolder>();
        _lights.OnLightTrigger += OnLightTrigger;
        transform.position = PatrolPoint1;
        _movingTo2 = true;
        _anim = GetComponent<Animator>();
        _dead = false;
        _rend = GetComponent<SpriteRenderer>();
        _boxCol = GetComponent<BoxCollider2D>();
        _diving = false;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _diveCooldown = float.NegativeInfinity;
    }

    private void Update()
    {
        if (_dead) return;
        _center = WorldCenter;
        Vector2 playerPos = PlayerCenter;
        (ILightSource, float) lightdist = nearestLight();
        ILightSource nearest = lightdist.Item1;
        Vector2 mag = playerPos - _center;
        Vector2 scaledMag = new Vector2(mag.x * DiveScale.x, mag.y * DiveScale.y);
        float playerSqr = scaledMag.sqrMagnitude;
        float dist = lightdist.Item2;
        if(!_diving)
            if (nearest.TurnedOn && dist < ShinyRange) Shiny(nearest);
            else if (DiveRange * DiveRange > playerSqr) CheckDive();
            if(!_diving)
                if (AgroRange * AgroRange > playerSqr) Agro();
                else Patrol();
        UpdatePosition();
        UpdateDirection();
    }

    private void UpdatePosition() => transform.position += (Vector3)_velocity;

    private void UpdateDirection()
    {
        bool right = _velocity.x > 0;
        if (_right != right)
        {
            _right = right;
            Flip();
        }
    }

    private void Flip() => transform.rotation = Quaternion.AngleAxis(_right ? 180f : 0f, Vector3.up);

    private void Shiny(ILightSource light)
    {
        Vector2 point = light.WorldCenter;
        PatrolPoint2 = PatrolPoint1 = point;
        Vector3 difference = point - _center;
        float speed = ShinySpeed * Time.deltaTime;
        bool atDestination = difference.sqrMagnitude < speed * speed;
        _velocity = (atDestination ? difference : difference.normalized * speed);
    }

    private void Patrol()
    {
        _anim.SetBool("Agro", false);
        Vector2 point = _movingTo2 ? PatrolPoint2 : PatrolPoint1;
        Vector3 difference = point - _center;
        float speed = PatrolSpeed * Time.deltaTime;
        bool atDestination = difference.sqrMagnitude < speed * speed;
        _velocity = (atDestination ? difference : difference.normalized * speed);
        if (atDestination) _movingTo2 = !_movingTo2;
    }

    private void Agro()
    {
        _anim.SetBool("Agro", true);
        Vector2 point = _movingTo2 ? PatrolPoint2 : PatrolPoint1;
        Vector3 difference = point - _center;
        float speed = AgroSpeed * Time.deltaTime;
        bool atDestination = difference.sqrMagnitude < speed * speed;
        _velocity = (atDestination ? difference : difference.normalized * speed);
        if (atDestination) _movingTo2 = !_movingTo2;
    }

    private void CheckDive()
    {
        if (Time.time - _diveCooldown < DiveCooldown) return;
        if (_right && PlayerCenter.x < _center.x) return;
        if (!_right && PlayerCenter.x > _center.x) return;
        Dive();
    }

    private (ILightSource, float) nearestLight()
    {
        float minDist = float.PositiveInfinity;
        ILightSource minLight = null;
        foreach (ILightSource light in _lights)
        {
            float d = (light.WorldCenter - _center).sqrMagnitude;
            if (d < minDist)
            {
                minLight = light;
                minDist = d;
            }
        }
        return (minLight, Mathf.Sqrt(minDist));
    }

    public virtual void takeDamage(float damage)
    {
        Health -= damage;

        if (health <= 0)
        {
            _dead = true;
            _anim.SetBool("Dead", true);
        }
    }

    private void OnLightTrigger(ref Collider2D collider)
    {
        if (collider == _boxCol)
        {
            takeDamage(Health);
        }
    }

    private void Die() => Destroy(gameObject);

    private void Dive()
    {
        _diving = true;
        _anim.SetBool("Diving", true);
        StartCoroutine(AnimateDive());
    }

    private void StopDive()
    {
        _diving = false;
        _anim.SetBool("Diving", false);
    }

    private IEnumerator AnimateDive()
    {
        float startTime = Time.time;
        float endTime = startTime + DiveTime;
        Func<float, float> scaletime = (x) => (x - startTime) / DiveTime;
        Func<float, float> genangle = (x) => DiveAngle * Mathf.Sin(Mathf.PI * x);
        float angle = 0, lastangle;
        freezeDive = transform.position;
        Vector2 mag = PlayerCenter - freezeDive;
        float s;
        while (true)
        {
            float t = Time.time;
            if (t > endTime) break;
            lastangle = angle;
            s = scaletime(t);
            angle = genangle(s);
            transform.Rotate(Vector3.forward, angle - lastangle);
            transform.position = freezeDive + mag * s;
            yield return null;
        }
        _diveCooldown = Time.time;
        StopDive();
    }
}
