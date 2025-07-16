using UnityEngine;

public class TrailSpawner : MonoBehaviour
{
    // The prefab to spawn as a trail element.
    public GameObject trailPrefab;

    // How often (in seconds) to spawn a trail element.
    public float spawnInterval = 0.1f;

    // Optional: an offset for the spawned trail relative to this object's position.
    public Vector3 spawnOffset = Vector3.zero;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTrail();
            timer = 0f;
        }
    }

    void SpawnTrail()
    {
        if (trailPrefab != null)
        {
            // Instantiate a trail prefab at the current position (with an optional offset).
            Instantiate(trailPrefab, transform.position + spawnOffset, Quaternion.identity);
        }
    }
}
