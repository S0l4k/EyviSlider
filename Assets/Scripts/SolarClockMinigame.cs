using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolarClockMinigame : MonoBehaviour
{
    [SerializeField] private Image solarClockShadow;
    [SerializeField] private TMP_Text targetHourText;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private GameObject winPanel;

    [SerializeField] private float matchThreshold = 5f;
    [SerializeField] private float requiredHoldTime = 2f;
    [SerializeField] private int requiredMatches = 3;

    private int successfulMatches = 0;
    private float currentTargetHour;
    private float shadowRotation;
    private float currentHoldTime;
    private bool isInPosition;
    private bool won = false;

    private void OnEnable()
    {
        ResetGame();
    }

    void Start()
    {
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
                SuccessfulMatch();
            }
        }
    }

    void UpdateSolarClock(float sliderValue)
    {
        shadowRotation = sliderValue * 360f;
        solarClockShadow.transform.rotation = Quaternion.Euler(0, 0, -shadowRotation);

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(shadowRotation, currentTargetHour * 30f));
        bool nowInPosition = angleDifference < matchThreshold;

        if (nowInPosition != isInPosition)
        {
            isInPosition = nowInPosition;
            currentHoldTime = 0f;
        }
    }

    void SuccessfulMatch()
    {
        successfulMatches++;
        progressText.text = successfulMatches + "/" + requiredMatches;
        currentHoldTime = 0f;

        if (successfulMatches >= requiredMatches)
        {
            won = true;
            winPanel.SetActive(true);
            timeSlider.interactable = false;
        }
        else
        {
            SetNewTargetHour();
        }
    }

    void SetNewTargetHour()
    {
        currentTargetHour = Random.Range(1, 12); // Random hour between 1-11
        targetHourText.text = currentTargetHour.ToString("00") + ":00";
        isInPosition = false;
    }

    public void ResetGame()
    {
        successfulMatches = 0;
        progressText.text = "0/" + requiredMatches;
        winPanel.SetActive(false);
        timeSlider.interactable = true;
        SetNewTargetHour();
        timeSlider.value = 0f;
    }
}