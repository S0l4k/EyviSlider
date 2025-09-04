using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public WireManager wireManager;
    public BulbCollector bulbCollector;
    public LensCleaner lensCleaner;

    
    public bool AllMinigamesCompleted()
    {
        return wireManager != null && wireManager.IsCompleted()
            && bulbCollector != null && bulbCollector.IsCompleted()
            && lensCleaner != null && lensCleaner.IsCompleted();
    }
}
