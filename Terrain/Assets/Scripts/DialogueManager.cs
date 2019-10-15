﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// Based off: https://www.youtube.com/watch?v=_nRzoTzeyxU
public class DialogueManager : MonoBehaviour
{
    private Queue<string> conversation;

    public GameObject continueButton;
    public GameObject startButton;
    public GameObject overlay;

    public int tutorialStage;
    public GameObject arrowImage;
    public GameObject goldArrow;
    public GameObject greenArrow;
    public GameObject happiArrow;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    //public Animator boxAnimator;
    //public Animator avatarAnimator;

    private void Start()
    {
        conversation = new Queue<string>();
        //boxAnimator.SetBool("isOpen", true);
        //avatarAnimator.SetBool("isOpen", true);

    }

    public void StartDialogue(Dialogue dialogue)
    {

        Debug.Log("Start convo");

        nameText.text = dialogue.npcName;

        conversation.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            conversation.Enqueue(sentence);
        }


        DisplayNextSentence();

        startButton.SetActive(false);
        continueButton.SetActive(true);

    }

    // Display the text of the dialogue slowly
    IEnumerator GenerateSentence (string sentence)
    {
        dialogueText.text = "";

        foreach (char digit in sentence.ToCharArray())
        {
            dialogueText.text += digit;
            yield return null;
        }
    }


    public void DisplayNextSentence()
    {
        if (conversation.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (conversation.Count == 9 && tutorialStage == 2)
        {
            arrowImage.SetActive(true);
        } else if (conversation.Count == 7 && tutorialStage == 2)
        {
            arrowImage.SetActive(false);
            goldArrow.SetActive(true);
        } else if (conversation.Count == 5 && tutorialStage == 2)
        {
            goldArrow.SetActive(false);
            greenArrow.SetActive(true);
        } else if (conversation.Count == 2 && tutorialStage == 2)
        {
            greenArrow.SetActive(false);
            happiArrow.SetActive(true);
        }

        string sentence = conversation.Dequeue();
        // Make sure that if the user clicks "next", the last
        // animation terminates
        StopAllCoroutines(); 
        StartCoroutine(GenerateSentence(sentence));

    }

    private IEnumerator WaitForAnimation (Animator animator)
    {
        do
        {
            yield return null;
        } while (animator.isActiveAndEnabled);
    }

    public void EndDialogue()
    {
        Debug.Log("Ended convo");
        //boxAnimator.SetBool("isOpen", false);
        //avatarAnimator.SetBool("isOpen", false);


        TutorialController.Instance.tutorialIndex++;

        overlay.SetActive(false);
    }


}
