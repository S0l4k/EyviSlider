using UnityEngine;

public class TriggerDirectionChange : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int Direction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            EarthController Controller = Dependencies.Instance.GetDependancy<EarthController>();
            Controller.SetRotationDirection(Direction);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            EarthController Controller = Dependencies.Instance.GetDependancy<EarthController>();
            Controller.SetRotationDirection(0);

        }
    }
}
