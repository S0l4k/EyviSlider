using UnityEngine;

public class ArrowKeysCameraMovement : MonoBehaviour
{
    // Speed at which the camera moves horizontally.
    public float moveSpeed = 5f;

    void Update()
    {
        // Get horizontal input (arrow keys or A/D).
        float horizontal = Input.GetAxis("Horizontal");

        // Move the camera only along the x-axis.
        transform.Translate(new Vector3(horizontal, 0f, 0f) * moveSpeed * Time.deltaTime, Space.World);
    }
}
