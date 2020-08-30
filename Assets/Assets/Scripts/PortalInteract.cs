using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteract : MonoBehaviour
{
    [SerializeField] private GameObject portalInteract;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            portalInteract.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Credits");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            portalInteract.SetActive(false);
        }
    }
}
