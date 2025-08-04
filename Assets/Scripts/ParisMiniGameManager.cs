using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParisMiniGameManager : MonoBehaviour
{
    public bool didAlreadyMeet = false;
    public bool isInteracting = false;
    public bool miniGameFinished = false;
    public List<GameObject> ingridients = new List<GameObject>();
    public GameObject textBox;
    public TextMeshProUGUI textUI;
    public string greatingText;
    public string missingIngridients;
    public string allIngridients;
    public string finishedMigiame;
    public int ingridientsCount = 0;

    private void Start()
    {
        Dependencies.Instance.RegisterDependency<ParisMiniGameManager>(this);
        didAlreadyMeet = false;
        miniGameFinished=false;
        isInteracting=false;
        textBox.SetActive(false);
        ingridientsCount=0;

        foreach(GameObject ingr in ingridients)
        {
            ingr.SetActive(true);
        }
    }

    private void OnEnable()
    {
        Dependencies.Instance.RegisterDependency<ParisMiniGameManager>(this);
        didAlreadyMeet = false;
        miniGameFinished = false;
        isInteracting = false;
        textBox.SetActive(false);
        ingridientsCount = 0;

        foreach (GameObject ingr in ingridients)
        {
            ingr.SetActive(true);
        }
    }

    public void OnClickInteraction()
    {
        if (!didAlreadyMeet && !isInteracting && !miniGameFinished)
        {
            StartCoroutine(Talking(greatingText));
        }

        if (didAlreadyMeet && !isInteracting && !miniGameFinished)
        {
            switch (ingridientsCount)
            {
                case 4: StartCoroutine(Talking(allIngridients));
                    break;
                default: StartCoroutine(Talking(missingIngridients));
                    break;

            }
        }

        if(didAlreadyMeet && !isInteracting && miniGameFinished)
        {
            StartCoroutine(Talking(finishedMigiame));
        }
    }

    public IEnumerator Talking(string talkPhase)
    {
        didAlreadyMeet = true;
        isInteracting=true;
        textBox.SetActive(true);
        textUI.text = talkPhase;
        if(ingridientsCount == 4) { miniGameFinished = true; }
        yield return new WaitUntil(() => isInteracting ==false);
        textBox.SetActive(false);
    }

    public void addIngridient()
    {
        ingridientsCount++;
    }

    public void StopInteraction()
    {
        isInteracting = false;
    }
}
