using UnityEngine;
using UnityEngine.UI;

public class CityFocusManager3D : MonoBehaviour
{
    [System.Serializable]
    public class CityData
    {
        public string cityName;
        public Transform cityTransform;
        public GameObject waypointButton;
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
        
    }

    void Update()
    {
        foreach (var city in cities)
        {
            float Distance = Mathf.Abs(mainCamera.transform.position.x) - Mathf.Abs(city.cityTransform.transform.position.x);
           
            if (Mathf.Abs(Distance)<focusThreshold)
            {
               city.isCentered = true;
                
            }
            else { city.isCentered = false; }
                Debug.Log(city.cityName + " " + Distance + " " + focusThreshold + " " + city.isCentered);
            bool inCenter=city.isCentered;

            if (inCenter && city.isCentered) {
                Debug.Log($"[CENTER DETECT] Miasto w centrum: {city.cityName}");

            city.isCentered = inCenter;
            Animator WaypointAnim = city.waypointButton.GetComponent<Animator>();

            WaypointAnim.SetTrigger("Start");
            }
            else
            {
                Animator WaypointAnim = city.waypointButton.GetComponent<Animator>();

                WaypointAnim.SetTrigger("End");
            }
        }
    }

    public void OnCityClicked(CityData city)
    {
        Debug.Log($"[CLICK DETECT] Klikni�to miasto: {city.cityName}");

        isInMiniGame = true;
        Animator WaypointAnim = city.waypointButton.GetComponent<Animator>();
        WaypointAnim.SetTrigger("End");

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
