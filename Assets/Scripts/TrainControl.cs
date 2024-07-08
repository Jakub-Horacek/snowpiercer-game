using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;

public class trainControl : MonoBehaviour
{
    private Rigidbody rb;
    public float maxForwardSpeedKmh = 200.0f; // Max forward speed in km/h
    public float maxBackwardSpeedKmh = 100.0f; // Max backward speed in km/h
    public float forwardAcceleration = 20.0f; // Acceleration rate for forward movement in m/s^2
    public float backwardAcceleration = 10.0f; // Acceleration rate for backward movement in m/s^2
    [SerializeField, ReadOnly(true)]
    private float currentSpeedKmh = 0.0f; // Current speed in km/h
    private bool autopilot = false; // Autopilot mode flag

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        float moveVertical = 0.0f;

        // Toggle off autopilot when movement keys are pressed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (autopilot)
            {
                autopilot = false;
                Debug.Log("Autopilot disabled");
            }
        }

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
            Debug.Log("Autopilot " + (autopilot ? "enabled" : "disabled"));
        }
    }
}
