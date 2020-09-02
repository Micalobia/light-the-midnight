using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ILightSource
{
    event OnLightTriggerDelegate OnLightTrigger;
    bool TurnedOn { get; set; }
    Vector2 WorldCenter { get; }
    bool UseInteract { get; set; }
    InteractReceiver interactReceiver { get; set; }
}

public delegate void OnLightTriggerDelegate(ref Collider2D collider);
