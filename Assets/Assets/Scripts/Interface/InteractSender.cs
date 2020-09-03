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
    private GameObject _empty;
    private bool _used;
    public event OnInteractDelegate OnInteract;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _trigger = GetComponent<BoxCollider2D>();
        _empty = new GameObject();
        _empty.transform.SetParentClean(transform);
        _interact = Instantiate(InteractPrefab);
        _interact.transform.SetParentClean(_empty.transform);
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
        _interact.transform.localScale = Scale;
        //_empty.transform.position = transform.TransformPoint(Offset);
        if (!_used && _interact.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            _used = !Multiuse;
            OnInteract?.Invoke();
            if (!Multiuse) _interact.SetActive(false);
        }
    }
}

public delegate void OnInteractDelegate();