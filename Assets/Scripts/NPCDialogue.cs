using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NPCDialogueWithTextbox : MonoBehaviour
{
    [Header("UI Elements")]
    public Button npcButton;
    public GameObject textBoxPanel;
    public TMP_Text dialogueText;
    public Image backgroundImage;
    public Sprite newBackground;
    public GameObject hotels;

    [Header("Minigame Manager")]
    public MinigameManager minigameManager;

    private int dialogueStage = 0;


    private string[] initialDialogue = new string[]
    {
        "Yo! Hey kid, thank god you’re here!\nListen, we got a serious sitch — the torch on Lady Liberty’s gone dark. Without it, ships won't know how to get to the harbor. We’re talkin’ total chaos on the water!\nProblem is, I can’t fix this on my own — too much ground to cover, too little time.\nI need you to hit up some key spots around the city, get those power sources back online. Think you can handle it?\nCheck out Empire State for the wires, Times Square for the light lens, and Central Park... you’re gonna need some bulbs, capisce?\nOnce you got all that jazz sorted, come back to me. And hurry — this city don’t sleep, but it *sure as hell* needs lights."
    };

    private string[] notDoneDialogue = new string[]
    {
        "Whoa whoa whoa — not so fast, chief.\nYou ain’t done yet! Gotta fix all three sites before we can light up the big lady again.\nBack to work, c’mon! Time’s tickin’ and the harbor ain’t waitin’."
    };

    private string[] doneDialogue = new string[]
    {
        "Well I’ll be... you actually did it!\nAlright, stand back — I’m flippin’ the switch...\nThree, two, one... *boom!* There she glows!\nYou just saved the harbor, buddy. Bet the captains are breathin’ easy now."
    };

    private string[] finalDialogue = new string[]
    {
        "That’s it! Look at those ships — glidin’ in safe and sound.\nYou really came through for us, friend. From this moment on, you’re an honorary New Yorker. Yeah, even if you don’t fold your pizza right.\nStick around a bit, enjoy the city — you earned it.\nAnd hey... if any lights go out again, you know who to call."
    };

    void Start()
    {

        if (textBoxPanel != null)
            textBoxPanel.SetActive(false);

        if (npcButton != null)
            npcButton.onClick.AddListener(OnNPCClicked);

        dialogueStage = 0;
    }

    void OnNPCClicked()
    {
        if (textBoxPanel != null)
            textBoxPanel.SetActive(true);


        bool allMinigamesDone = (minigameManager != null && minigameManager.AllMinigamesCompleted());

        if (!allMinigamesDone)
        {

            dialogueText.text = (dialogueStage == 0) ? initialDialogue[0] : notDoneDialogue[0];
            dialogueStage = 1;
        }
        else
        {

            if (dialogueStage == 1)
            {
                dialogueText.text = doneDialogue[0];
                dialogueStage = 2;
            }
            else if (dialogueStage == 2)
            {

                if (backgroundImage != null && newBackground != null)
                    backgroundImage.sprite = newBackground;

                dialogueText.text = finalDialogue[0];
                dialogueStage = 3;
                StartCoroutine(showHotels());
            }
        }


    }
    public IEnumerator showHotels()
    {
        yield return new WaitForSeconds(10);

        hotels.SetActive(true);
    }
}

