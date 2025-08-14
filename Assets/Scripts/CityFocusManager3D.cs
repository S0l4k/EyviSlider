using UnityEngine;
using UnityEngine.UI;

public class CityFocusManager3D : MonoBehaviour
{
    [System.Serializable]
    public class CityData
    {
        public string cityName;
        public Transform cityTransform;
        public Button waypointButtonPrefab;
        public Canvas cityMiniGameCanvas;

        [HideInInspector] public Button waypointInstance;
        [HideInInspector] public bool isCentered;
    }

    public Camera mainCamera;
    public float focusThreshold = 0.05f;
    public RectTransform uiParent;
    public CityData[] cities;

    [HideInInspector] public bool isInMiniGame = false;

    void Start()
    {
        foreach (var city in cities)
        {
            if (city.waypointButtonPrefab != null)
            {
                city.waypointInstance = Instantiate(city.waypointButtonPrefab, uiParent, false);
                city.waypointInstance.gameObject.SetActive(false);
                city.waypointInstance.onClick.AddListener(() => OnCityClicked(city));
            }

            if (city.cityMiniGameCanvas != null)
                city.cityMiniGameCanvas.gameObject.SetActive(false);

            city.isCentered = false;
        }
    }

    void Update()
    {
        foreach (var city in cities)
        {
            Vector3 screenPos = mainCamera.WorldToViewportPoint(city.cityTransform.position);
            float dx = Mathf.Abs(screenPos.x - 0.5f);
            float dy = Mathf.Abs(screenPos.y - 0.5f);

            bool inCenter = dx < focusThreshold && dy < focusThreshold && screenPos.z > 0;

            if (inCenter && !city.isCentered)
                Debug.Log($"[CENTER DETECT] Miasto w centrum: {city.cityName}");

            city.isCentered = inCenter;

            
            if (city.waypointInstance != null)
            {
                city.waypointInstance.gameObject.SetActive(city.isCentered && !isInMiniGame);
                city.waypointInstance.transform.position = mainCamera.WorldToScreenPoint(city.cityTransform.position);
            }
        }
    }

    void OnCityClicked(CityData city)
    {
        Debug.Log($"[CLICK DETECT] Klikniêto miasto: {city.cityName}");

        isInMiniGame = true;

        
        foreach (var otherCity in cities)
        {
            if (otherCity.cityMiniGameCanvas != null)
                otherCity.cityMiniGameCanvas.gameObject.SetActive(false);
        }

        
        if (city.cityMiniGameCanvas != null)
            city.cityMiniGameCanvas.gameObject.SetActive(true);
    }

    
    public void ExitMiniGame()
    {
        isInMiniGame = false;

    }
}
