using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightSourceHolder : MonoBehaviour, ILightSource, IEnumerable<ILightSource>
{
    public event OnLightTriggerDelegate OnLightTrigger
    {
        add
        {
            foreach (LightSourceLine _ in _a) _.OnLightTrigger += value;
            foreach (LightSourcePoint _ in _b) _.OnLightTrigger += value;
            foreach (LightSourceHolder _ in _c) _.OnLightTrigger += value;
        }
        remove
        {
            foreach (LightSourceLine _ in _a) _.OnLightTrigger -= value;
            foreach (LightSourcePoint _ in _b) _.OnLightTrigger -= value;
            foreach (LightSourceHolder _ in _c) _.OnLightTrigger -= value;
        }
    }
    public bool TurnedOn
    {
        get
        {
            foreach (LightSourceLine _ in _a) if (_.TurnedOn) return true;
            foreach (LightSourcePoint _ in _b) if (_.TurnedOn) return true;
            foreach (LightSourceHolder _ in _c) if (_.TurnedOn) return true;
            return false;
        }
        set
        {
            foreach (LightSourceLine _ in _a) _.TurnedOn = value;
            foreach (LightSourcePoint _ in _b) _.TurnedOn = value;
            foreach (LightSourceHolder _ in _c) _.TurnedOn = value;
        }
    }
    public Vector2 WorldCenter => transform.TransformPoint(transform.position);
    [SerializeField] private bool Interactable;
    public bool UseInteract { get => Interactable; set => Interactable = value; }
    public InteractReceiver interactReceiver { get; set; }

    private LightSourceLine[] _a;
    private LightSourcePoint[] _b;
    private LightSourceHolder[] _c;

    private void Awake()
    {
        _a = GetComponentsInChildren<LightSourceLine>();
        _b = GetComponentsInChildren<LightSourcePoint>();
        List<LightSourceHolder> c = GetComponentsInChildren<LightSourceHolder>().ToList();
        c.Remove(this);
        _c = c.ToArray();
        if (UseInteract)
        {
            interactReceiver = GetComponent<InteractReceiver>();
            interactReceiver.OnInteract += () => TurnedOn = !TurnedOn;
        }
    }

    public IEnumerator<ILightSource> GetEnumerator()
    {
        List<ILightSource> ret = new List<ILightSource>();
        ret.AddRange(_a);
        ret.AddRange(_b);
        ret.AddRange(_c);
        return ret.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}