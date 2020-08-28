using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Globalization;

public class UsernameManager: MonoBehaviour

{
    public GameObject inputField;

    // Player name variable and property to access
    // it from other scripts.
    static string playerName;
    public static string PlayerName
    {
        get { return playerName; }
        set { Debug.Log("You are not allowed to set the player name like that"); }
    }

    //Use this on a "Submit" button to set the playerName variable.
    public void SubmitName()
    {
        {
            playerName = inputField.GetComponent<Text>().text;
        }
    }
}
