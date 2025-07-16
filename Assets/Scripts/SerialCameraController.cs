using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;

public class SerialCameraController : MonoBehaviour
{
    [Header("Serial Port Settings")]
    public string portName = "COM4";  // Default port; can be changed at runtime.
    public int baudRate = 9600;

    [Header("Camera Movement Boundaries")]
    public SpriteRenderer boundarySprite;
    // Assign the camera you want to control via serial.
    public Camera controlledCamera;

    // Optionally override boundaries from sprite with custom values.
    [Header("Custom Boundary Settings")]
    public bool useCustomBoundaries = false;
    public float customLeftBound;
    public float customRightBound;

    [Header("Camera Movement Smoothing")]
    // Adjust this value to control how fast the camera moves toward the target.
    public float smoothingSpeed = 5f;
    // Internal target x value that the camera will smoothly move toward.
    private float targetCameraX;

    [Header("Invert Camera Movement")]
    [Tooltip("If checked, the camera movement will be inverted (serial input is reversed).")]
    public bool invertMovement = false;

    private SerialPort serialPort;
    private Thread serialThread;
    private bool isRunning = false;

    // Queue to store incoming serial messages.
    private Queue<string> messageQueue = new Queue<string>();

    // Persistent encoder range values (for the lifetime of the application).
    private float dynamicEncoderMin = float.PositiveInfinity;
    private float dynamicEncoderMax = float.NegativeInfinity;

    // Updated in Update() when parsing a new encoder value.
    public float lastEncoderValue = 0f;

    // Flag to toggle window visibility
    private bool showWindow = false;

    // Position and size of the window
    private Rect windowRect = new Rect(100, 100, 250, 150);

    // --- PlayerPrefs Keys ---
    private const string KEY_SERIAL_PORT = "SerialPort";
    private const string KEY_CUSTOM_LEFT_BOUND = "CustomLeftBound";
    private const string KEY_CUSTOM_RIGHT_BOUND = "CustomRightBound";
    private const string KEY_DYNAMIC_ENCODER_MIN = "DynamicEncoderMin";
    private const string KEY_DYNAMIC_ENCODER_MAX = "DynamicEncoderMax";
    private const string KEY_SMOOTHING_SPEED = "SmoothingSpeed";
    private const string KEY_INVERT_MOTION = "InvertMotion";







    public void Recalibrate()
    {
        dynamicEncoderMin = lastEncoderValue;
        dynamicEncoderMax = lastEncoderValue;
        Debug.Log("Recalibrated min and max to " + lastEncoderValue);
    }

    void Start()
    {
        LoadSettings();
        OpenSerialPort();

        // Use controlledCamera if assigned; otherwise, fallback to Camera.main.
        if (controlledCamera == null)
        {
            controlledCamera = Camera.main;
            Debug.LogWarning("Controlled Camera not assigned; falling back to Camera.main");
        }
        // Initialize targetCameraX with the current camera x position.
        if (controlledCamera != null)
            targetCameraX = controlledCamera.transform.position.x;
    }

    void OpenSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }

        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 100;

        try
        {
            serialPort.Open();
            isRunning = true;
            serialThread = new Thread(ReadSerial);
            serialThread.Start();
            Debug.Log("Opened serial port: " + portName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error opening serial port: " + ex.Message);
        }
    }

    // Public method to update the COM port at runtime.
    public void SetPortName(string newPort)
    {
        // Check if the new port is the same as the current one.
        if (newPort == portName)
        {
            Debug.LogWarning("The selected COM port is already in use. No changes made.");
            return;
        }

        portName = newPort;
        Debug.Log("Changing port to: " + portName);
        OpenSerialPort();
    }

    // Public method to set custom boundaries.
    public void SetCustomBoundaries(float left, float right)
    {
        customLeftBound = left;
        customRightBound = right;
        useCustomBoundaries = true;
        Debug.Log($"Custom boundaries set. Left: {left}, Right: {right}");
    }

    void ReadSerial()
    {
        while (isRunning)
        {
            try
            {
                string message = serialPort.ReadLine();
                lock (messageQueue)
                {
                    messageQueue.Enqueue(message);
                }
            }
            catch (System.TimeoutException)
            {
                // Continue reading.
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Serial read error: " + ex.Message);
            }
        }
    }

    void Update()
    {
        // Toggle the window on/off when the L key is pressed.
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    showWindow = !showWindow;
        //}
        // Process all queued serial messages.
        lock (messageQueue)
        {
            while (messageQueue.Count > 0)
            {
                string message = messageQueue.Dequeue();
                Debug.Log("Received: " + message + " MIN: " + dynamicEncoderMin + " MAX: " + dynamicEncoderMax);

                if (float.TryParse(message.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float encoderValue))
                {
                    lastEncoderValue = encoderValue;

                    if (encoderValue < dynamicEncoderMin && encoderValue != 0)
                    {
                        dynamicEncoderMin = encoderValue;
                        Debug.Log("Updated min: " + dynamicEncoderMin);
                    }
                    if (encoderValue > dynamicEncoderMax)
                    {
                        dynamicEncoderMax = encoderValue;
                        Debug.Log("Updated max: " + dynamicEncoderMax);
                    }

                    float range = dynamicEncoderMax - dynamicEncoderMin;
                    float t = (range > Mathf.Epsilon)
                        ? Mathf.InverseLerp(dynamicEncoderMin, dynamicEncoderMax, encoderValue)
                        : 0.5f;

                    // Determine boundaries: use custom if enabled; otherwise, derive from sprite.
                    float leftBound, rightBound;
                    if (useCustomBoundaries)
                    {
                        leftBound = customLeftBound;
                        rightBound = customRightBound;
                    }
                    else if (boundarySprite != null && controlledCamera != null)
                    {
                        float camHalfWidth = controlledCamera.orthographicSize * controlledCamera.aspect;
                        leftBound = boundarySprite.bounds.min.x + camHalfWidth;
                        rightBound = boundarySprite.bounds.max.x - camHalfWidth;
                    }
                    else
                    {
                        // Fall back to using encoderValue directly.
                        leftBound = encoderValue - 1f;
                        rightBound = encoderValue + 1f;
                    }

                    // Invert movement if needed.
                    float newX = invertMovement
                        ? Mathf.Lerp(leftBound, rightBound, 1 - t)
                        : Mathf.Lerp(leftBound, rightBound, t);
                    targetCameraX = newX;
                    // Debug.Log($"Camera recalculated: targetX={targetCameraX}, leftBound={leftBound}, rightBound={rightBound}, t={t}");
                }
                else
                {
                    Debug.LogWarning("Unable to parse encoder value from message: " + message);
                }
            }
        }

        // Smoothly move the controlled camera toward the target X position.
        if (controlledCamera != null)
        {
            Vector3 currentPos = controlledCamera.transform.position;
            float smoothX = Mathf.Lerp(currentPos.x, targetCameraX, smoothingSpeed * Time.deltaTime);
            controlledCamera.transform.position = new Vector3(smoothX, currentPos.y, currentPos.z);
        }
    }

    // LateUpdate is used here to snap the camera's position to full pixels.
    void LateUpdate()
    {
        if (controlledCamera != null)
        {
            // Calculate the world units per pixel.
            float unitsPerPixel = (controlledCamera.orthographicSize * 2) / Screen.height;
            Vector3 pos = controlledCamera.transform.position;

            // Snap X and Y to the nearest whole pixel.
            pos.x = Mathf.Round(pos.x / unitsPerPixel) * unitsPerPixel;
            pos.y = Mathf.Round(pos.y / unitsPerPixel) * unitsPerPixel;
            controlledCamera.transform.position = pos;
        }
    }

    void OnGUI()
    {
        // If showWindow is true, display the window.
        if (showWindow)
        {
            windowRect = GUI.Window(0, windowRect, DrawWindowContents, "Settings");
        }
    }

    // This function draws the contents of the window.
    void DrawWindowContents(int windowID)
    {
        GUILayout.BeginVertical();

        // Display current smoothing speed value.
        GUILayout.Label("Smoothing Speed: " + smoothingSpeed.ToString("F2"));
        smoothingSpeed = GUILayout.HorizontalSlider(smoothingSpeed, 0f, 20f);

        // Checkbox to invert camera movement.
        invertMovement = GUILayout.Toggle(invertMovement, "Invert Camera Movement");

        // Button to close the window.
        if (GUILayout.Button("Close"))
        {
            showWindow = false;
        }

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join();
        }
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
        SaveSettings();
    }

    void SaveSettings()
    {
        PlayerPrefs.SetString(KEY_SERIAL_PORT, portName);
        PlayerPrefs.SetFloat(KEY_CUSTOM_LEFT_BOUND, customLeftBound);
        PlayerPrefs.SetFloat(KEY_CUSTOM_RIGHT_BOUND, customRightBound);
        PlayerPrefs.SetFloat(KEY_DYNAMIC_ENCODER_MIN, dynamicEncoderMin);
        PlayerPrefs.SetFloat(KEY_DYNAMIC_ENCODER_MAX, dynamicEncoderMax);
        PlayerPrefs.SetFloat(KEY_SMOOTHING_SPEED, smoothingSpeed);
        PlayerPrefs.SetInt(KEY_INVERT_MOTION, invertMovement ? 1 : 0);


        PlayerPrefs.Save();
        Debug.Log("Settings saved.");
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey(KEY_SERIAL_PORT))
        {
            portName = PlayerPrefs.GetString(KEY_SERIAL_PORT);
        }
        if (PlayerPrefs.HasKey(KEY_CUSTOM_LEFT_BOUND))
        {
            customLeftBound = PlayerPrefs.GetFloat(KEY_CUSTOM_LEFT_BOUND);
            useCustomBoundaries = true;
        }
        if (PlayerPrefs.HasKey(KEY_CUSTOM_RIGHT_BOUND))
        {
            customRightBound = PlayerPrefs.GetFloat(KEY_CUSTOM_RIGHT_BOUND);
            useCustomBoundaries = true;
        }
        if (PlayerPrefs.HasKey(KEY_DYNAMIC_ENCODER_MIN))
        {
            dynamicEncoderMin = PlayerPrefs.GetFloat(KEY_DYNAMIC_ENCODER_MIN);
        }
        if (PlayerPrefs.HasKey(KEY_DYNAMIC_ENCODER_MAX))
        {
            dynamicEncoderMax = PlayerPrefs.GetFloat(KEY_DYNAMIC_ENCODER_MAX);
        }
        if (PlayerPrefs.HasKey(KEY_SMOOTHING_SPEED))
        {
            smoothingSpeed = PlayerPrefs.GetFloat(KEY_SMOOTHING_SPEED);
        }
        if (PlayerPrefs.HasKey(KEY_INVERT_MOTION))
        {
            invertMovement = PlayerPrefs.GetInt(KEY_INVERT_MOTION) == 1;
        }
        Debug.Log("Settings loaded.");
    }
}
