using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class RecallUsername : MonoBehaviour
{
    public Text textDisplay;

    void Start()
    {
        string name = UsernameManager.PlayerName;
        textDisplay.text = name;
    }
}
