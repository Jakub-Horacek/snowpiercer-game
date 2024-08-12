using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class trainControl : MonoBehaviour
{
    [Header("Train Components")]
    private Rigidbody rb;
    [Header("Train Control")]
    public float maxForwardSpeedKmh = 200.0f; // Max forward speed in km/h
    public float maxBackwardSpeedKmh = 100.0f; // Max backward speed in km/h
    public float forwardAcceleration = 20.0f; // Acceleration rate for forward movement in m/s^2
    public float backwardAcceleration = 10.0f; // Acceleration rate for backward movement in m/s^2
    [SerializeField, ReadOnly(true)]
    private float currentSpeedKmh = 0.0f; // Current speed in km/h
    private bool autopilot = false; // Autopilot mode flag

    [Header("HUD")]
    public Color forwardColor = Color.blue; // #31FCFE
    public Color backwardColor = Color.red; // #872727
    public Image ForwardSpeedometer;
    public Image BackwardSpeedometer;
    public TextMeshProUGUI SpeedText;
    public GameObject AutopilotIndicator;

    [Header("Spawn Manager")]
    public SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        float moveVertical = 0.0f;

        // Convert current speed from km/h to m/s for calculations
        float currentSpeedMs = KmhToMs(currentSpeedKmh);

        if (!autopilot)
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveVertical = 1.0f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveVertical = -1.0f;
            }

            // If not in autopilot mode, handle manual acceleration/deceleration
            if (moveVertical > 0)
            {
                // Accelerate towards the max forward speed
                currentSpeedMs = Mathf.Min(currentSpeedMs + forwardAcceleration * Time.fixedDeltaTime, KmhToMs(maxForwardSpeedKmh));
            }
            else if (moveVertical < 0)
            {
                // Accelerate towards the max backward speed
                currentSpeedMs = Mathf.Max(currentSpeedMs - backwardAcceleration * Time.fixedDeltaTime, -KmhToMs(maxBackwardSpeedKmh));
            }
            else
            {
                // Gradually decelerate to 0 if no movement input
                if (currentSpeedMs > 0)
                {
                    currentSpeedMs = Mathf.Max(currentSpeedMs - forwardAcceleration * Time.fixedDeltaTime, 0);
                }
                else if (currentSpeedMs < 0)
                {
                    currentSpeedMs = Mathf.Min(currentSpeedMs + backwardAcceleration * Time.fixedDeltaTime, 0);
                }
            }
        }
        else
        {
            // If in autopilot mode, maintain current speed
            moveVertical = Mathf.Sign(currentSpeedMs);
            currentSpeedMs = Mathf.Clamp(currentSpeedMs, -KmhToMs(maxBackwardSpeedKmh), KmhToMs(maxForwardSpeedKmh));
        }

        // Convert current speed from m/s to km/h for display and storage
        currentSpeedKmh = MsToKmh(currentSpeedMs);
        UpdateSpeedometer(currentSpeedKmh);

        // Apply the current speed to the train's velocity
        Vector3 velocity = rb.velocity;
        velocity.z = currentSpeedMs;
        rb.velocity = velocity;
    }

    // Helper method to convert km/h to m/s
    float KmhToMs(float kmh)
    {
        return kmh * 1000.0f / 3600.0f;
    }

    // Helper method to convert m/s to km/h
    float MsToKmh(float ms)
    {
        return ms * 3600.0f / 1000.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle autopilot mode on/off with key 'P'
        if (Input.GetKeyDown(KeyCode.P))
        {
            autopilot = !autopilot;
            AutopilotIndicator.SetActive(autopilot);
        }

        if (autopilot && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)))
        {
            autopilot = false;
            AutopilotIndicator.SetActive(false);
        }
    }

    void UpdateSpeedometer(float speed)
    {
        if (speed >= 0)
        {
            ForwardSpeedometer.fillAmount = speed / maxForwardSpeedKmh;
            BackwardSpeedometer.fillAmount = 0;
        }
        else
        {
            ForwardSpeedometer.fillAmount = 0;
            BackwardSpeedometer.fillAmount = -speed / maxBackwardSpeedKmh;
        }

        SpeedText.text = Mathf.Abs(speed).ToString("F0");
        SpeedText.color = speed >= 0 ? forwardColor : backwardColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        spawnManager.SpawnTriggerEntered(currentSpeedKmh, other.name);
    }
}
