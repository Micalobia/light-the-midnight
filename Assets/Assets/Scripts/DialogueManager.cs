using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
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
    public AudioSource audioSource;
    public GameObject trigger;
    public GameObject newTrigger;
    public GameObject newManager;

    private Queue<string> sentences;
    private Queue<AudioClip> clips;


    private void Start()
    {
        newTrigger.SetActive(false);
        newManager.SetActive(false);
    }

    void Awake()
    {
        sentences = new Queue<string>();
        clips = new Queue<AudioClip>();
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
        foreach (AudioClip clip in dialogue.clips)
        {
            clips.Enqueue(clip);
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
        AudioClip clip = clips.Dequeue();
        Debug.Log(clip);
        Debug.Log(audioSource);
        audioSource.PlayOneShot(clip);
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
        trigger.SetActive(false);
        Player.SetActive(true);
        pressSpace.SetActive(false);
        newTrigger.SetActive(true);
        newManager.SetActive(true);
    }

}
