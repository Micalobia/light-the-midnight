using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsAnim : MonoBehaviour
{
    [SerializeField]
    private Canvas ThankYou_Canvas;

    [SerializeField]
    private Canvas Starring_Canvas;

    [SerializeField]
    private Canvas Animation_Canvas;

    [SerializeField]
    private Canvas Environment_Canvas;

    [SerializeField]
    private Canvas Programming_Canvas;

    [SerializeField]
    private Canvas Music_Canvas;

    [SerializeField]
    private Canvas About_Canvas;

    [SerializeField]
    private Canvas About2_Canvas;

    public void Awake()
    {

        Starring_Canvas.gameObject.SetActive(false);
        Animation_Canvas.gameObject.SetActive(false);
        Environment_Canvas.gameObject.SetActive(false);
        Programming_Canvas.gameObject.SetActive(false);
        Music_Canvas.gameObject.SetActive(false);
        About_Canvas.gameObject.SetActive(false);
        About2_Canvas.gameObject.SetActive(false);
    }

    private void ChangeThankYou()
    {
        ThankYou_Canvas.gameObject.SetActive(false);
        Starring_Canvas.gameObject.SetActive(true);

    }
    private void ChangeStarring()
    {
        Starring_Canvas.gameObject.SetActive(false);
        Animation_Canvas.gameObject.SetActive(true);

    }
    private void ChangeAnimation()
    {
        Animation_Canvas.gameObject.SetActive(false);
        Environment_Canvas.gameObject.SetActive(true);

    }
    private void ChangeEnvironment()
    {
        Environment_Canvas.gameObject.SetActive(false);
        Programming_Canvas.gameObject.SetActive(true);

    }
    private void ChangeProgramming()
    {
        Programming_Canvas.gameObject.SetActive(false);
        Music_Canvas.gameObject.SetActive(true);

    }
    private void ChangeMusic()
    {
        Music_Canvas.gameObject.SetActive(false);
        About_Canvas.gameObject.SetActive(true);

    }
    private void ChangeAbout()
    {
        About_Canvas.gameObject.SetActive(false);
        About2_Canvas.gameObject.SetActive(true);

    }

    public void toMenu()
    {
    SceneManager.LoadScene("MainMenu");
    }
}