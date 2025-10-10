using UnityEngine;
using UnityEngine.UI;

public class ConnectionPoint : MonoBehaviour
{
    public GameObject trach;
    public string wireColor; 
    public bool isConnected = false;

    private void Update()
    {
        if(isConnected)
        { trach.SetActive(true); }
    }
}
