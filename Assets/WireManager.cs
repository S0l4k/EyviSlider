using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WireManager : MonoBehaviour
{
    public Camera uiCamera; 
    public LineRenderer wirePrefab; 
    public Transform drawArea; 

    private ConnectionPoint selectedPoint;
    private LineRenderer currentLine;
    private List<(ConnectionPoint, ConnectionPoint)> connections = new List<(ConnectionPoint, ConnectionPoint)>();

    void Update()
    {
       
        if (selectedPoint != null && currentLine != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -19f;
            Vector3 worldMousePos = uiCamera.ScreenToWorldPoint(mousePos);
            worldMousePos.z = -19f;
            currentLine.SetPosition(1, worldMousePos);
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(uiCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                ConnectionPoint cp = hit.collider.GetComponent<ConnectionPoint>();
                if (cp != null && !cp.isConnected)
                {
                    selectedPoint = cp;
                    StartWire(selectedPoint);
                }
            }
        }

        
        if (Input.GetMouseButtonUp(0) && selectedPoint != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(uiCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                ConnectionPoint cp = hit.collider.GetComponent<ConnectionPoint>();

                if (cp != null && cp != selectedPoint && !cp.isConnected && cp.wireColor == selectedPoint.wireColor)
                {
                    
                    FinishWire(cp);
                    selectedPoint.isConnected = true;
                    cp.isConnected = true;
                    connections.Add((selectedPoint, cp));
                    selectedPoint = null;

                    CheckWin();
                }
                else
                {
                    
                    Destroy(currentLine.gameObject);
                    selectedPoint = null;
                }
            }
            else
            {
                Destroy(currentLine.gameObject);
                selectedPoint = null;
            }
        }
    }

    void StartWire(ConnectionPoint startPoint)
    {
        currentLine = Instantiate(wirePrefab, drawArea);
        currentLine.positionCount = 2;

        Vector3 startWorld = startPoint.GetComponent<RectTransform>().position;
        startWorld.z = -19f;

        currentLine.SetPosition(0, startWorld);
        currentLine.SetPosition(1, startWorld);
    }

    void FinishWire(ConnectionPoint endPoint)
    {
        Vector3 endWorld = endPoint.GetComponent<RectTransform>().position;
        endWorld.z = -19f;

        currentLine.SetPosition(1, endWorld);
        currentLine = null;
    }

    void CheckWin()
    {
        ConnectionPoint[] allPoints = FindObjectsOfType<ConnectionPoint>();
        foreach (var point in allPoints)
        {
            if (!point.isConnected) return;
        }
        Debug.Log(" Wszystkie kable połączone elo benc");
    }
}
