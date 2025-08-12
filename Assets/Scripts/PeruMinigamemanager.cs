using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class PeruMinigamemanager : MonoBehaviour
{
    public GameObject DatesBlock;
    public GameObject winScreen;
    public List<GameObject> targets;
    public List<GameObject> dates;
    public float wiggleValue;
    public int currentScore;
    public bool Minigame1 = false;
    public bool Minigame2 = false;
    public bool Minigame3 = false;
    public bool Minigame4 = false;

    private void Start()
    {
        Dependencies.Instance.RegisterDependency<PeruMinigamemanager>(this);

    }
    private void OnEnable()
    {
        Minigame1 = false;
        Minigame2 = false;
        Minigame3 = false;
        Minigame4 = false;
        currentScore = 0;
        foreach (var _dates in dates)
        {
            _dates.transform.SetLocalPositionAndRotation(new Vector3(-1.2f, Random.Range(-0.6f, 1f), 0), Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        if(Minigame1 && Minigame2 && Minigame3 && Minigame4)
        {
            DatesBlock.SetActive(false);
        }
    }

    public void ChechDates()
    {
        currentScore = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            float currentDistance = Vector3.Distance(targets[i].transform.position, dates[i].transform.position);
            if (currentDistance <= wiggleValue) currentScore++;
        }
        if (currentScore == 4) StartCoroutine(winning());
    }


    public IEnumerator winning()
    {
        yield return new WaitForSeconds(2);

        winScreen.SetActive(true);
    }

}
