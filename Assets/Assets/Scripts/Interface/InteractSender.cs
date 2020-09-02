using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractSender : MonoBehaviour
{
    [SerializeField] private Vector2 Offset;
    [SerializeField] private Vector2 Scale;
    [SerializeField] private GameObject InteractPrefab;
    [SerializeField] private bool Multiuse;
    private GameObject _player;
    private BoxCollider2D _trigger;
    private GameObject _interact;
    private bool _used;
    public event OnInteractDelegate OnInteract;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _trigger = GetComponent<BoxCollider2D>();
        _interact = Instantiate(InteractPrefab);
        _interact.transform.SetParentClean(transform);
        _interact.transform.position = Offset;
        _interact.transform.localScale = Scale;
        _interact.SetActive(false);
        _used = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player) _interact.SetActive(!_used);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player) _interact.SetActive(false);
    }

    private void Update()
    {
        if (!_used && _interact.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            _used = !Multiuse;
            OnInteract?.Invoke();
        }
    }
}

public delegate void OnInteractDelegate();