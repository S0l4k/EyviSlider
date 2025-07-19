using UnityEngine;

public class EarthController : MonoBehaviour
{
    public Transform[] rotationWaypoints;
    public float rotationSpeed = 0.8f;
    public float rotationSmoothness = 5f;
    public int RotationDirection = 0;
    private float currentProgress = 0f;
    private Quaternion targetRotation;

    private void Awake()
    {
        Dependencies.Instance.RegisterDependency<EarthController>(this);
    }
    void Update()
    {
        
        currentProgress = Mathf.Clamp01(currentProgress + RotationDirection * rotationSpeed * Time.deltaTime);

        targetRotation = CalculateTargetRotation(currentProgress);
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                             targetRotation,
                                             rotationSmoothness * Time.deltaTime);
    }

    Quaternion CalculateTargetRotation(float progress)
    {
        if (rotationWaypoints.Length == 0) return transform.rotation;
        if (rotationWaypoints.Length == 1) return rotationWaypoints[0].rotation;

        float totalPoints = rotationWaypoints.Length - 1;
        float exactIndex = progress * totalPoints;
        int indexA = Mathf.FloorToInt(exactIndex);
        int indexB = Mathf.Min(indexA + 1, rotationWaypoints.Length - 1);
        float t = exactIndex - indexA;

        return Quaternion.Slerp(rotationWaypoints[indexA].rotation,
                               rotationWaypoints[indexB].rotation,
                               t);
    }

    // Optional path visualization
    void OnDrawGizmosSelected()
    {
        if (rotationWaypoints == null || rotationWaypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        Vector3 earthPosition = transform.position;
        for (int i = 0; i < rotationWaypoints.Length - 1; i++)
        {
            Vector3 start = earthPosition + rotationWaypoints[i].position;
            Vector3 end = earthPosition + rotationWaypoints[i + 1].position;
            Gizmos.DrawLine(start, end);
        }
    }
    

    public void SetRotationDirection( int dir)
    {
        RotationDirection = dir;
    }
}
    
