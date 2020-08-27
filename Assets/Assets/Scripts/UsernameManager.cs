using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class UsernameManager: MonoBehaviour

{
    private void Start()
    {
        nameField = GetComponent<InputField>();
    }
    public InputField nameField;

    // Player name variable and property to access
    // it from other scripts.
    string playerName;
    public string PlayerName
    {
        get { return playerName; }
        set { Debug.Log("You are not allowed to set the player name like that"); }
    }

    //Use this on a "Submit" button to set the playerName variable.
    public void SubmitName()
    {
        if (string.IsNullOrEmpty(nameField.text) == false)
        {
            playerName = nameField.text;
        }
    }
}
