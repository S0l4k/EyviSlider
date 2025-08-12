using UnityEngine;

public class WindowChech : MonoBehaviour
{
    public string Tag;

    private void Start()
    {
       
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag))
        {
            ThreeWindowsMiniGame windowsMiniGame = Dependencies.Instance.GetDependancy<ThreeWindowsMiniGame>();
            windowsMiniGame.ItemsInPlace++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tag))
        {
            ThreeWindowsMiniGame windowsMiniGame = Dependencies.Instance.GetDependancy<ThreeWindowsMiniGame>();
            windowsMiniGame.ItemsInPlace--;
        }
    }
}
