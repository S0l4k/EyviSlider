using UnityEngine;

public class RecalibrateMinMaxOnKeyPress : MonoBehaviour
{
    void Update()
    {
        // Check if Ctrl and Shift are held and the user presses 'R'
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        // if (ctrl && shift && Input.GetKeyDown(KeyCode.R))
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Use the new recommended method to find any instance of the SerialCameraControlPersistentRange
            SerialCameraController controlScript = Object.FindAnyObjectByType<SerialCameraController>();
            if (controlScript != null)
            {
                controlScript.Recalibrate();
                Debug.Log("Recalibration triggered via Ctrl+Shift+R.");
            }
            else
            {
                Debug.LogWarning("No SerialCameraControlPersistentRange instance found.");
            }
        }
    }
}
