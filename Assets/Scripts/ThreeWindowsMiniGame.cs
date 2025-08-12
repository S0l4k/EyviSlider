using TMPro;
using UnityEngine;

public class ThreeWindowsMiniGame : MonoBehaviour
{
    public int ItemsInPlace = 0;
    public GameObject WinScreen;
    public TextMeshProUGUI Counter;

    void Start()
    {
        ItemsInPlace = 0;
      Dependencies.Instance.RegisterDependency<ThreeWindowsMiniGame>(this);
    }



    public void CheckWindows()
    {
        Counter.text = ItemsInPlace + "/9";
    }
}
