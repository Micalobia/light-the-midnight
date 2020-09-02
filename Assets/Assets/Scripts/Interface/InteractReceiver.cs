using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InteractReceiver : MonoBehaviour
{
    [SerializeField] private InteractSender Sender;
    public event OnInteractDelegate OnInteract
    {
        add => Sender.OnInteract += value;
        remove => Sender.OnInteract -= value;
    }
}
