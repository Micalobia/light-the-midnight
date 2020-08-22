using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoAnim : MonoBehaviour
{

    [SerializeField]
    private Image jamLogo;

    [SerializeField]
    private Image Presents;

    [SerializeField]
    private Canvas logoCanvas;


    [SerializeField]
    private Canvas MainMenuCanvas;

    public void Awake()
    {

        MainMenuCanvas.gameObject.SetActive(false);
    }

    private void ChangeCanvas()
    {
        logoCanvas.gameObject.SetActive(false);
        MainMenuCanvas.gameObject.SetActive(true);

    }
    private void destroySprite()
    {
        Destroy(jamLogo);
    }

}
