using UnityEngine;

public class BindDeltaXPosition : MonoBehaviour
{
    // The target whose x position change drives the movement.
    public Transform target;

    // The factor by which the target's x movement is applied.
    // For example, if factor is 0.5, the GameObject will move half as fast as the target.
    public float factor = 1f;

    // To store the previous x position of the target.
    private float previousTargetX;

    void Start()
    {
        if (target != null)
        {
            // Record the initial x position of the target.
            previousTargetX = target.position.x;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate the change in the target's x position since the last frame.
            float deltaX = target.position.x - previousTargetX;

            // Update only the x-coordinate of this GameObject's position.
            Vector3 currentPos = transform.position;
            currentPos.x += deltaX * factor;
            transform.position = currentPos;

            // Update previousTargetX for the next frame.
            previousTargetX = target.position.x;
        }
    }
}
