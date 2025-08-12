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
    void Enable()
    {
        ItemsInPlace = 0;
      Dependencies.Instance.RegisterDependency<ThreeWindowsMiniGame>(this);
    }



    public void CheckWindows()
    {
        Counter.text = ItemsInPlace + "/9";

        if(ItemsInPlace == 9)
        {
            WinScreen.SetActive(true);
        }
    }
}
