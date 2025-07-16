using UnityEngine;

public class DeactivateParentOnClick : MonoBehaviour
{
    public string targetTag = "YourTagHere"; // Set the tag in the Inspector or assign it in code

    // This method is called when the object is clicked.
    void OnMouseDown()
    {
        EnableAllBoxColliders();
        if (transform.parent != null)
        {
            // Deactivate the parent GameObject.
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No parent found for " + gameObject.name);
        }
    }

    public void EnableAllBoxColliders()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject obj in objects)
        {
            BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.enabled = true; // Enable only the BoxCollider
            }
        }
    }
}
