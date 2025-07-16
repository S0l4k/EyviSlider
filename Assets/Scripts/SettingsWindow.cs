using UnityEngine;
using UnityEngine.Video; // If needed for other references
using System.IO.Ports;  // Required to get serial port names

public class SettingsWindow : MonoBehaviour
{
    [Header("Script References")]
    public SerialCameraController serialCameraController;
    public IdleFadeActivator idleFadeActivator;
    public GUISkin skin;

    // Toggle the settings window
    private bool showWindow = false;

    // The position/size of the GUI window (x, y, width, height)
    // Increased height to accommodate the extra buttons.
    private Rect windowRect = new Rect(100, 100, 1500, 2000);

    // Scale factor for making the entire GUI larger (2x)
    private float scaleFactor = 2f;

    // List of available serial ports and selected index
    private string[] availablePorts = new string[0];
    private int selectedPortIndex = 0;

    // Boundary step value for adjustment (default is 1)
    private float boundaryStep = 1f;

    private void Start()
    {
        // Query active serial ports (Windows only)
#if UNITY_STANDALONE_WIN
        availablePorts = SerialPort.GetPortNames();
        if (availablePorts == null || availablePorts.Length == 0)
        {
            availablePorts = new string[] { "No Ports" };
        }
#else
        availablePorts = new string[] { "N/A" };
#endif

        // If the serialCameraController already has a portName, try to match it
        if (serialCameraController != null && !string.IsNullOrEmpty(serialCameraController.portName))
        {
            for (int i = 0; i < availablePorts.Length; i++)
            {
                if (availablePorts[i] == serialCameraController.portName)
                {
                    selectedPortIndex = i;
                    break;
                }
            }
        }
    }

    private void Update()
    {
        // Toggle the settings window when the user presses 'S'
        if (Input.GetKeyDown(KeyCode.S))
        {
            showWindow = !showWindow;
        }
    }

    private void OnGUI()
    {
        if (showWindow)
        {
            GUI.skin = skin;
            // Store the current GUI matrix and color so we can restore them later
            Matrix4x4 oldMatrix = GUI.matrix;

            // Create a scaled rectangle so the window appears in the same spot on screen,
            // but drawn at twice the size
            Rect scaledRect = new Rect(
                windowRect.x / scaleFactor,
                windowRect.y / scaleFactor,
                windowRect.width / scaleFactor,
                windowRect.height / scaleFactor
            );

            // Scale the GUI by 'scaleFactor'
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1f));


            // Draw the settings window at the scaled position
            scaledRect = GUI.Window(0, scaledRect, DrawSettingsWindow, "Settings");

            // Revert the GUI matrix and color to avoid affecting other UI
            //GUI.matrix = oldMatrix;

            // Convert back to unscaled coordinates so dragging the window feels correct
            windowRect = new Rect(
                scaledRect.x * scaleFactor,
                scaledRect.y * scaleFactor,
                scaledRect.width * scaleFactor,
                scaledRect.height * scaleFactor
            );
        }
    }

    /// <summary>
    /// The method that draws the contents of our GUI window.
    /// </summary>
    private void DrawSettingsWindow(int windowID)
    {
        // -------------------------------
        // 1) SerialCameraController
        // -------------------------------
        if (serialCameraController != null)
        {
            // Last Encoder Value (Read-Only)
            GUILayout.Label("Last Encoder Value: " + serialCameraController.lastEncoderValue);
            GUILayout.Space(10);

            GUILayout.Label("<b>Serial Camera Controller</b>", GetBoldStyle());

            // Serial Port rectangular buttons
            GUILayout.Label("Serial Port:");
            if (availablePorts != null && availablePorts.Length > 0)
            {
                for (int i = 0; i < availablePorts.Length; i++)
                {
                    // Highlight selected port with a different background color
                    GUI.backgroundColor = (i == selectedPortIndex) ? Color.green : Color.white;
                    if (GUILayout.Button(availablePorts[i], GUILayout.Height(30)))
                    {
                        selectedPortIndex = i;
                        serialCameraController.portName = availablePorts[i];
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUILayout.Label("No serial ports available");
            }

            GUILayout.Space(10);

            // Use Custom Boundaries
            GUILayout.Label("Use Custom Boundaries:");
            serialCameraController.useCustomBoundaries = GUILayout.Toggle(serialCameraController.useCustomBoundaries, "Enabled");
            if (serialCameraController.useCustomBoundaries)
            {
                // Left Boundary with + and - buttons
                GUILayout.Label("Left Boundary:");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(40)))
                {
                    serialCameraController.customLeftBound -= boundaryStep;
                }
                GUILayout.Label(serialCameraController.customLeftBound.ToString("F2"), GUILayout.Width(80));
                if (GUILayout.Button("+", GUILayout.Width(40)))
                {
                    serialCameraController.customLeftBound += boundaryStep;
                }
                GUILayout.EndHorizontal();

                // Right Boundary with + and - buttons
                GUILayout.Label("Right Boundary:");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(40)))
                {
                    serialCameraController.customRightBound -= boundaryStep;
                }
                GUILayout.Label(serialCameraController.customRightBound.ToString("F2"), GUILayout.Width(80));
                if (GUILayout.Button("+", GUILayout.Width(40)))
                {
                    serialCameraController.customRightBound += boundaryStep;
                }
                GUILayout.EndHorizontal();

                // Step selection buttons for boundary adjustment
                GUILayout.Label("Step Value:");
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = (boundaryStep == 1f) ? Color.green : Color.white;
                if (GUILayout.Button("1", GUILayout.Width(40))) { boundaryStep = 1f; }
                GUI.backgroundColor = (boundaryStep == 0.1f) ? Color.green : Color.white;
                if (GUILayout.Button("0.1", GUILayout.Width(40))) { boundaryStep = 0.1f; }
                GUI.backgroundColor = (boundaryStep == 0.01f) ? Color.green : Color.white;
                if (GUILayout.Button("0.01", GUILayout.Width(40))) { boundaryStep = 0.01f; }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }

            // Smoothing Speed as a slider (0 to 40)
            GUILayout.Label("Smoothing Speed: " + serialCameraController.smoothingSpeed.ToString("F2"));
            serialCameraController.smoothingSpeed = GUILayout.HorizontalSlider(serialCameraController.smoothingSpeed, 0f, 40f);
            // Buttons for Smoothing Speed presets: 0, 5, 10, 20, 40
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0", GUILayout.Width(40))) { serialCameraController.smoothingSpeed = 0f; }
            if (GUILayout.Button("5", GUILayout.Width(40))) { serialCameraController.smoothingSpeed = 5f; }
            if (GUILayout.Button("10", GUILayout.Width(40))) { serialCameraController.smoothingSpeed = 10f; }
            if (GUILayout.Button("20", GUILayout.Width(40))) { serialCameraController.smoothingSpeed = 20f; }
            if (GUILayout.Button("40", GUILayout.Width(40))) { serialCameraController.smoothingSpeed = 40f; }
            GUILayout.EndHorizontal();

            // Invert Movement
            GUILayout.Label("Invert Movement:");
            serialCameraController.invertMovement = GUILayout.Toggle(serialCameraController.invertMovement, "Enabled");
        }
        else
        {
            GUILayout.Label("No SerialCameraController assigned.");
        }

        GUILayout.Space(10);

        // -------------------------------
        // 2) IdleFadeActivator
        // -------------------------------
        if (idleFadeActivator != null)
        {
            GUILayout.Label("<b>Idle Fade Activator</b>", GetBoldStyle());

            // Idle Time
            GUILayout.Label("Idle Time:");
            idleFadeActivator.idleTime = FloatField(idleFadeActivator.idleTime);
            // Buttons for Idle Time presets: 5, 10, 30, 60, 120
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("5", GUILayout.Width(40))) { idleFadeActivator.idleTime = 5f; }
            if (GUILayout.Button("10", GUILayout.Width(40))) { idleFadeActivator.idleTime = 10f; }
            if (GUILayout.Button("30", GUILayout.Width(40))) { idleFadeActivator.idleTime = 30f; }
            if (GUILayout.Button("60", GUILayout.Width(40))) { idleFadeActivator.idleTime = 60f; }
            if (GUILayout.Button("120", GUILayout.Width(40))) { idleFadeActivator.idleTime = 120f; }
            GUILayout.EndHorizontal();

            // Fade In Duration
            GUILayout.Label("Fade In Duration:");
            idleFadeActivator.fadeInDuration = FloatField(idleFadeActivator.fadeInDuration);
            // Buttons for Fade In presets: 0.3, 0.5, 1, 1.5, 2
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.3", GUILayout.Width(40))) { idleFadeActivator.fadeInDuration = 0.3f; }
            if (GUILayout.Button("0.5", GUILayout.Width(40))) { idleFadeActivator.fadeInDuration = 0.5f; }
            if (GUILayout.Button("1", GUILayout.Width(40))) { idleFadeActivator.fadeInDuration = 1f; }
            if (GUILayout.Button("1.5", GUILayout.Width(40))) { idleFadeActivator.fadeInDuration = 1.5f; }
            if (GUILayout.Button("2", GUILayout.Width(40))) { idleFadeActivator.fadeInDuration = 2f; }
            GUILayout.EndHorizontal();

            // Fade Out Duration
            GUILayout.Label("Fade Out Duration:");
            idleFadeActivator.fadeOutDuration = FloatField(idleFadeActivator.fadeOutDuration);
            // Buttons for Fade Out presets: 0.3, 0.5, 1, 1.5, 2
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.3", GUILayout.Width(40))) { idleFadeActivator.fadeOutDuration = 0.3f; }
            if (GUILayout.Button("0.5", GUILayout.Width(40))) { idleFadeActivator.fadeOutDuration = 0.5f; }
            if (GUILayout.Button("1", GUILayout.Width(40))) { idleFadeActivator.fadeOutDuration = 1f; }
            if (GUILayout.Button("1.5", GUILayout.Width(40))) { idleFadeActivator.fadeOutDuration = 1.5f; }
            if (GUILayout.Button("2", GUILayout.Width(40))) { idleFadeActivator.fadeOutDuration = 2f; }
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("No IdleFadeActivator assigned.");
        }

        GUILayout.Space(10);

        // Close button
        if (GUILayout.Button("Close"))
        {
            showWindow = false;
        }

        // Make the window draggable
        GUI.DragWindow();
    }

    /// <summary>
    /// Safely parse a float from a text field.
    /// </summary>
    private float FloatField(float value)
    {
        string valString = GUILayout.TextField(value.ToString());
        if (float.TryParse(valString, out float result))
        {
            return result;
        }
        return value;
    }

    /// <summary>
    /// Returns a GUIStyle for bold labels (optional).
    /// </summary>
    private GUIStyle GetBoldStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.richText = true;
        return style;
    }
}
