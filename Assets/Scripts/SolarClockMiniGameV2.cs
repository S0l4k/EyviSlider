using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolarClockMiniGameV2 : MonoBehaviour
{
    public Slider timeSlider;
    public List<Sprite> shadows;
    public List <Sprite> nidles;
    public Image currentShadow;
    public Image currentNidle;
    public TMP_Text targetHourText;
    public TMP_Text progressText;
    public GameObject winPanel;
    private int requiredMatches = 3;
    private int successfulMatches = 0;
    private int currentTargetHour;
    public int currentGuessedHour;
    private float currentHoldTime = 0;
    private float requiredHoldTime = 2;
    private bool won = false;
    private bool isInPosition = false;

    private void OnEnable()
    {
        ResetGame();
        winPanel.SetActive(false);
        timeSlider.onValueChanged.AddListener(UpdateSolarClock);
    }

    void Update()
    {
        if (isInPosition && !won)
        {
            currentHoldTime += Time.deltaTime;
            if (currentHoldTime >= requiredHoldTime)
            {
                RoundWon();
            }
        }
    }


    void SetNewTargetHour()
    {
        currentTargetHour = Random.Range(1, 18); // Random hour between 1-11

        if(currentTargetHour < 9)
        {
            targetHourText.text = currentTargetHour.ToString("00") + ":00 PM";
        }
        else
        {
            targetHourText.text = (currentTargetHour - 5).ToString("00") + ":00 AM";
        }
    }

    public void UpdateSolarClock(float sliderValue)
    {
        if (sliderValue > 0.05 && sliderValue <= 0.10) { currentNidle.sprite = nidles[1]; currentShadow.sprite = shadows[1]; currentGuessedHour = 1; } // 4am
        else if (sliderValue > 0.10 && sliderValue <= 0.15) { currentNidle.sprite = nidles[1]; currentShadow.sprite = shadows[2]; currentGuessedHour = 2; } // 5am
        else if (sliderValue > 0.15 && sliderValue <= 0.20) { currentNidle.sprite = nidles[1]; currentShadow.sprite = shadows[3]; currentGuessedHour = 3; } // 6am
        else if (sliderValue > 0.20 && sliderValue <= 0.25) { currentNidle.sprite = nidles[2]; currentShadow.sprite = shadows[4]; currentGuessedHour = 4; } // 7am
        else if (sliderValue > 0.25 && sliderValue <= 0.30) { currentNidle.sprite = nidles[2]; currentShadow.sprite = shadows[5]; currentGuessedHour = 5; } // 8am
        else if (sliderValue > 0.30 && sliderValue <= 0.35) { currentNidle.sprite = nidles[2]; currentShadow.sprite = shadows[6]; currentGuessedHour = 6; } // 9am
        else if (sliderValue > 0.35 && sliderValue <= 0.40) { currentNidle.sprite = nidles[3]; currentShadow.sprite = shadows[7]; currentGuessedHour = 7; } // 10am
        else if (sliderValue > 0.40 && sliderValue <= 0.45) { currentNidle.sprite = nidles[3]; currentShadow.sprite = shadows[8]; currentGuessedHour = 8; } // 11am
        else if (sliderValue > 0.45 && sliderValue <= 0.50) { currentNidle.sprite = nidles[3]; currentShadow.sprite = shadows[9]; currentGuessedHour = 9; } // 12am
        else if (sliderValue > 0.50 && sliderValue <= 0.55) { currentNidle.sprite = nidles[3]; currentShadow.sprite = shadows[10]; currentGuessedHour = 10; } // 1pm
        else if (sliderValue > 0.55 && sliderValue <= 0.60) { currentNidle.sprite = nidles[4]; currentShadow.sprite = shadows[11]; currentGuessedHour = 11; } // 2pm
        else if (sliderValue > 0.60 && sliderValue <= 0.65) { currentNidle.sprite = nidles[4]; currentShadow.sprite = shadows[12]; currentGuessedHour = 12; } // 3pm
        else if (sliderValue > 0.65 && sliderValue <= 0.70) { currentNidle.sprite = nidles[4]; currentShadow.sprite = shadows[13]; currentGuessedHour = 13; } // 4pm
        else if (sliderValue > 0.70 && sliderValue <= 0.75) { currentNidle.sprite = nidles[4]; currentShadow.sprite = shadows[14]; currentGuessedHour = 14; } // 5pm
        else if (sliderValue > 0.75 && sliderValue <= 0.80) { currentNidle.sprite = nidles[5]; currentShadow.sprite = shadows[15]; currentGuessedHour = 15; } // 6pm
        else if (sliderValue > 0.80 && sliderValue <= 0.85) { currentNidle.sprite = nidles[5]; currentShadow.sprite = shadows[16]; currentGuessedHour = 16; } // 7pm
        else if (sliderValue > 0.85 && sliderValue <= 0.90) { currentNidle.sprite = nidles[5]; currentShadow.sprite = shadows[17]; currentGuessedHour = 17; } // 8pm
        else if (sliderValue > 0.90 && sliderValue <= 0.95) { currentNidle.sprite = nidles[0]; currentShadow.sprite = shadows[0]; currentGuessedHour = -1; } // night
        else{ currentNidle.sprite = nidles[0]; currentShadow.sprite = shadows[0]; currentGuessedHour = -1; } // night

        if(currentTargetHour == currentGuessedHour)
        {
            currentHoldTime = 0f;
            isInPosition = true;
        }
        else { isInPosition = false; }
    }

    public void RoundWon()
    {
        successfulMatches++;
        progressText.text =successfulMatches + "/" + requiredMatches;
        if (successfulMatches == requiredMatches)
        {
            winPanel.SetActive(true);
            timeSlider.interactable = false;
            PeruMinigamemanager PeruMainGame = Dependencies.Instance.GetDependancy<PeruMinigamemanager>();
            PeruMainGame.Minigame2 = true;
            won = true;
        }
        else { SetNewTargetHour(); }
    }


    public void ResetGame()
    {
        successfulMatches = 0;
        progressText.text = "0/" + requiredMatches;
        winPanel.SetActive(false);
        timeSlider.interactable = true;
        SetNewTargetHour();
        timeSlider.value = 0f;
        won = false;
    }

}
