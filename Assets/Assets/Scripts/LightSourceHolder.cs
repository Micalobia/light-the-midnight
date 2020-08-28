using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightSourceHolder : MonoBehaviour, ILightSource
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
    }
}