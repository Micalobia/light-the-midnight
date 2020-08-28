using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    public float volume = 0.75f;
    public float delay;
    public float time;
    public Text dialogueText;
    bool nextText;
    bool playing;
    public Animator animator;
    public GameObject dialogueBox;
    public GameObject Player;
    public GameObject pressSpace;

    private Queue<string> sentences;

    // Start is called before the first frame update
    void Awake()
    {
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Player.SetActive(false);
        pressSpace.SetActive(true);
        animator.SetBool("IsOpen", true);
        sentences.Clear();
        playing = true;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);

        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence ()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    //char animations
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            yield return new WaitForSeconds(0.05f);
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        dialogueBox.SetActive(false);
        Player.SetActive(true);
        pressSpace.SetActive(false);
    }

}
