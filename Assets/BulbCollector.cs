using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class BulbCollector : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] bulbs;      
    public TMP_Text messageText; 

    private int foundCount = 0;

    void Start()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);

       
        for (int i = 0; i < bulbs.Length; i++)
        {
            int index = i; 
            bulbs[i].onClick.AddListener(() => OnBulbClicked(index));
        }
    }

    private bool completed = false;
    void OnBulbClicked(int index)
    {
        
        bulbs[index].gameObject.SetActive(false);

       
        foundCount++;

       
        if (foundCount >= bulbs.Length)
        {
            if (messageText != null)
            {
                completed = true;
                messageText.gameObject.SetActive(true);
                messageText.text = "You did it!";
            }
        }
    }
    public bool IsCompleted()
    {
        return completed;
    }
}
