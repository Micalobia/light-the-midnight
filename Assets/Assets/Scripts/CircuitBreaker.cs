using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBreaker : MonoBehaviour
{
    [SerializeField] private Animator streetLamp;
    [SerializeField] private GameObject interact;
    [SerializeField] private BoxCollider2D interactBox;
   private void Awake()
   {
        streetLamp = GetComponentInParent<Animator>();
        interactBox = GetComponent<BoxCollider2D>();
        interact.SetActive(false);

   }

    private void Update()
    {
        ActiveLight();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interact.SetActive(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interact.SetActive(false);
        }
    }

    void ActiveLight()
    {
        if (interact.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            interact.SetActive(false);
            streetLamp.SetTrigger("turnedOff");
            interactBox.enabled = false;
        }
    }


}
