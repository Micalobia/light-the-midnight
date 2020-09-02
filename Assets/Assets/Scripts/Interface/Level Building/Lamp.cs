using UnityEngine;

[RequireComponent(typeof(LightSourceHolder))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Lamp : MonoBehaviour
{
    [SerializeField] private bool Interactable;
    
    private Animator _anim;
    private InteractReceiver _rec;

    [SerializeField] private bool _on;

    private void Start()
    {
        _on = GetComponent<LightSourceHolder>().TurnedOn;
        _anim = GetComponent<Animator>();
        if (Interactable)
        {
            _rec = GetComponent<InteractReceiver>();
            _rec.OnInteract += () => _on = !_on;
        }
    }

    private void Update() => _anim.SetBool("On", _on);
}
