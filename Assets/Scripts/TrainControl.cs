using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainControl : MonoBehaviour
{
    // Enum to define the type of deceleration
    public enum DecelerationType
    {
        Quadratic,
        Exponential
    }

    private Rigidbody rb;

    [Header("Train Control")]
    [SerializeField]
    private float maxForwardSpeedKmh = 200.0f; // Max forward speed in km/h
    [SerializeField]
    private float maxBackwardSpeedKmh = 100.0f; // Max backward speed in km/h
    [SerializeField]
    private float forwardAcceleration = 20.0f; // Acceleration rate for forward movement in km/h per second
    [SerializeField]
    private float backwardAcceleration = 10.0f; // Acceleration rate for backward movement in km/h per second
    private float currentSpeedKmh = 0.0f; // Current speed in km/h
    private bool autopilot = false; // Autopilot mode flag

    [Header("Deceleration")]
    [SerializeField]
    [Tooltip("Quadratic Deceleration is stronger at higher speeds, offering a more gradual slowdown as speed decreases.\n\nExponential Deceleration reduces speed more significantly, providing a more aggressive slow-down as speed increases.")]
    private DecelerationType decelerationType = DecelerationType.Quadratic; // Dropdown for selecting deceleration type

    [Header("HUD")]
    [SerializeField]
    [Tooltip("Default - #31FCFE")]
    private Color forwardColor = new Color(0.192f, 0.988f, 0.996f, 1.0f);
    [SerializeField]
    [Tooltip("Default - #872727")]
    private Color backwardColor = new Color(0.529f, 0.153f, 0.153f, 1.0f);
    [SerializeField]
    private Image forwardSpeedometer;
    [SerializeField]
    private Image backwardSpeedometer;
    [SerializeField]
    private TextMeshProUGUI speedText;
    [SerializeField]
    private GameObject autopilotIndicator;

    [Header("Etc")]
    [SerializeField]
    private SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called once per frame for physics-based calculations
    void FixedUpdate()
    {
        float moveVertical = 0.0f;

        if (!autopilot)
        {
            // Detect input for forward and backward movement
            if (Input.GetKey(KeyCode.W))
            {
                moveVertical = 1.0f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveVertical = -1.0f;
            }

            // Handle acceleration or braking
            if (moveVertical > 0)
            {
                // Accelerate towards max forward speed
                currentSpeedKmh = Mathf.Min(currentSpeedKmh + forwardAcceleration * Time.fixedDeltaTime, maxForwardSpeedKmh);
            }
            else if (moveVertical < 0)
            {
                // Accelerate towards max backward speed
                currentSpeedKmh = Mathf.Max(currentSpeedKmh - backwardAcceleration * Time.fixedDeltaTime, -maxBackwardSpeedKmh);
            }
            else
            {
                // Apply passive deceleration when no input is pressed
                currentSpeedKmh = ApplyDeceleration(currentSpeedKmh);
            }
        }
        else
        {
            // Maintain speed in autopilot mode
            currentSpeedKmh = Mathf.Clamp(currentSpeedKmh, -maxBackwardSpeedKmh, maxForwardSpeedKmh);
        }

        // Apply braking when holding the opposite key
        if (moveVertical != Mathf.Sign(currentSpeedKmh) && moveVertical != 0)
        {
            currentSpeedKmh = ApplyDeceleration(currentSpeedKmh);
        }

        // Apply the current speed to the train's velocity
        Vector3 velocity = rb.velocity;
        velocity.z = KmhToMs(currentSpeedKmh); // Convert km/h to m/s
        rb.velocity = velocity;

        UpdateSpeedometer(currentSpeedKmh);
    }

    // Convert km/h to m/s
    private float KmhToMs(float kmh)
    {
        return kmh * 1000.0f / 3600.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle autopilot mode on/off with key 'P'
        if (Input.GetKeyDown(KeyCode.P))
        {
            autopilot = !autopilot;
            autopilotIndicator.SetActive(autopilot);
        }

        // Disable autopilot if manual control keys are pressed
        if (autopilot && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)))
        {
            autopilot = false;
            autopilotIndicator.SetActive(false);
        }
    }

    // Method to update the HUD speedometer display
    private void UpdateSpeedometer(float speed)
    {
        if (speed >= 0)
        {
            forwardSpeedometer.fillAmount = speed / maxForwardSpeedKmh;
            backwardSpeedometer.fillAmount = 0;
        }
        else
        {
            forwardSpeedometer.fillAmount = 0;
            backwardSpeedometer.fillAmount = -speed / maxBackwardSpeedKmh;
        }

        speedText.text = Mathf.Abs(speed).ToString("F0");
        speedText.color = speed >= 0 ? forwardColor : backwardColor;
    }

    // Method to handle deceleration based on the selected deceleration type
    private float ApplyDeceleration(float speedKmh)
    {
        float decelerationRate;

        // Determine deceleration rate based on selected deceleration type
        if (decelerationType == DecelerationType.Quadratic)
        {
            decelerationRate = forwardAcceleration * (1 - Mathf.Abs(speedKmh) / maxForwardSpeedKmh);
        }
        else // Exponential Deceleration
        {
            decelerationRate = forwardAcceleration * Mathf.Exp(-Mathf.Abs(speedKmh) / maxForwardSpeedKmh);
        }

        // Apply deceleration to bring the train to a stop or reverse
        if (speedKmh > 0)
        {
            speedKmh = Mathf.Max(speedKmh - decelerationRate * Time.fixedDeltaTime, 0);
        }
        else if (speedKmh < 0)
        {
            speedKmh = Mathf.Min(speedKmh + decelerationRate * Time.fixedDeltaTime, 0);
        }

        return speedKmh;
    }

    // Handle collision with spawn triggers
    private void OnTriggerEnter(Collider other)
    {
        spawnManager.SpawnTriggerEntered(currentSpeedKmh, other.name);
    }
}
