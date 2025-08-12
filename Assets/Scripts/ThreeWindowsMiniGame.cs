using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThreeWindowsMiniGame : MonoBehaviour
{
    public int ItemsInPlace = 0;
    public GameObject WinScreen;
    public TextMeshProUGUI Counter;
    public List<GameObject> Items;
    public float dispersionValueX;
    public float dispersionValueY;
    private void OnEnable()
    {
        ItemsInPlace = 0;
        foreach (var item in Items)
        {
            item.transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-dispersionValueX, dispersionValueX), Random.Range(-dispersionValueY, dispersionValueY), 0), Quaternion.identity);
        }

        WinScreen.SetActive(false);
        Dependencies.Instance.RegisterDependency<ThreeWindowsMiniGame>(this);
    }


    public void CheckWindows()
    {
        Counter.text = ItemsInPlace + "/9";

        if(ItemsInPlace == 9)
        {
            WinScreen.SetActive(true);
            PeruMinigamemanager PeruMainGame = Dependencies.Instance.GetDependancy<PeruMinigamemanager>();
            PeruMainGame.Minigame3 = true;
        }
    }
}
