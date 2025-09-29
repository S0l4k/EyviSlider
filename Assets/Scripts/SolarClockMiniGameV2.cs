using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolarClockMiniGameV2 : MonoBehaviour
{
    public Slider TimeSlider;
    public List<Sprite> Shadows;
    public Image CurrentShadow;
    public TMP_Text targetHourText;
    public TMP_Text progressText;
    public GameObject winPanel;
    private int requiredMatches = 3;
    private int successfulMatches = 0;
    private float currentTargetHour;
    private bool won = false;

    private void OnEnable()
    {
        ResetGame();
    }


    void SetNewTargetHour()
    {
        currentTargetHour = Random.Range(2, 20); // Random hour between 1-11
        targetHourText.text = currentTargetHour.ToString("00") + ":00";
    }




    public void ResetGame()
    {
        successfulMatches = 0;
        progressText.text = "0/" + requiredMatches;
        winPanel.SetActive(false);
        TimeSlider.interactable = true;
        SetNewTargetHour();
        TimeSlider.value = 0f;
        won = false;
    }

}
