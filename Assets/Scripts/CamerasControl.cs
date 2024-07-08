using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    public CameraData[] cameraData;

    [System.Serializable]
    public class CameraData
    {
        public GameObject camera;
        public bool canOrbit;
    }

    public Transform cameraOrbitPoint;
    public float rotationSpeed = 5f; // Adjust the rotation speed as needed
    public float minDistance = 15f; // Minimum distance from the orbit point
    public float maxDistance = 150f; // Maximum distance from the orbit point
    public float collisionOffset = 0.5f; // Offset to prevent clipping

    private int activeCameraIndex = 0;
    private bool isOrbiting = false; // Flag to track if orbiting is active

    private Quaternion initialOrbitPointRotation; // Store initial rotation of cameraOrbitPoint
    private float currentDistance; // Current distance from the orbit point

    // Start is called before the first frame update
    void Start()
    {
        // Disable all cameras except the first one
        for (int i = 0; i < cameraData.Length; i++)
        {
            cameraData[i].camera.SetActive(i == activeCameraIndex);
        }

        // Store initial rotation of cameraOrbitPoint
        initialOrbitPointRotation = cameraOrbitPoint.rotation;

        // Initialize the current distance to max distance
        currentDistance = maxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameras();
        }

        for (int i = 0; i < cameraData.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetActiveCamera(i);
            }
        }

        // Handle camera orbiting
        if (cameraData[activeCameraIndex].canOrbit)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                OrbitCamera();
                isOrbiting = true;
            }
            else if (isOrbiting)
            {
                ResetCameraOrbit();
                isOrbiting = false;
            }

            // Check for collisions and adjust camera distance
            CheckForCollisionsAndAdjustDistance();

            // Adjust camera distance based on scroll input
            float scrollInput = Input.mouseScrollDelta.y;
            if (scrollInput != 0.0f)
            {
                currentDistance = Mathf.Clamp(currentDistance - scrollInput * rotationSpeed, minDistance, maxDistance);
                UpdateCameraPosition();
            }
        }
    }

    void ToggleCameras()
    {
        // Toggle to the next camera
        SetActiveCamera((activeCameraIndex + 1) % cameraData.Length);
    }

    void SetActiveCamera(int cameraIndex)
    {
        // Reset cameraOrbitPoint rotation when switching cameras
        ResetCameraOrbit();

        // Deactivate the current camera
        cameraData[activeCameraIndex].camera.SetActive(false);
        // Update active camera index
        activeCameraIndex = cameraIndex;
        // Activate the new active camera
        cameraData[activeCameraIndex].camera.SetActive(true);

        // Reset the distance to maxDistance when switching cameras
        currentDistance = maxDistance;
    }

    void OrbitCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 rotation = new Vector3(-mouseY, mouseX, 0f) * rotationSpeed;

        // Rotate camera orbit point (parent of all cameras) around world axes
        cameraOrbitPoint.Rotate(Vector3.up, rotation.y, Space.World); // Rotate around Y-axis
        cameraOrbitPoint.Rotate(cameraOrbitPoint.right, rotation.x, Space.World); // Rotate around X-axis

        // Adjust camera distance with scroll wheel
        float scrollInput = Input.mouseScrollDelta.x;
        if (scrollInput != 0.0f)
        {
            // Adjust current distance based on scroll input
            currentDistance = Mathf.Clamp(currentDistance - scrollInput * rotationSpeed, minDistance, maxDistance);
            UpdateCameraPosition();
        }
    }

    void ResetCameraOrbit()
    {
        cameraOrbitPoint.rotation = initialOrbitPointRotation; // Reset rotation to initial rotation
        currentDistance = maxDistance; // Reset distance to max distance
    }

    void CheckForCollisionsAndAdjustDistance()
    {
        if (!cameraData[activeCameraIndex].canOrbit) return;

        GameObject activeCamera = cameraData[activeCameraIndex].camera;

        // Calculate the direction from the cameraOrbitPoint to the activeCamera
        Vector3 direction = activeCamera.transform.position - cameraOrbitPoint.position;
        float distance = direction.magnitude;

        // Raycast from the cameraOrbitPoint to the activeCamera
        if (Physics.Raycast(cameraOrbitPoint.position, direction, out RaycastHit hit, maxDistance))
        {
            // If there's a collision, adjust the camera distance
            if (hit.collider.gameObject != activeCamera)
            {
                distance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, maxDistance);
            }
            else
            {
                // No collision, restore the camera distance to the desired distance
                distance = currentDistance;
            }
        }
        else
        {
            // No collision, restore the camera distance to the desired distance
            distance = currentDistance;
        }

        // Update camera position
        activeCamera.transform.position = cameraOrbitPoint.position + direction.normalized * distance;
    }

    void UpdateCameraPosition()
    {
        if (!cameraData[activeCameraIndex].canOrbit) return;

        GameObject activeCamera = cameraData[activeCameraIndex].camera;

        // Calculate the direction from the cameraOrbitPoint to the activeCamera
        Vector3 direction = (activeCamera.transform.position - cameraOrbitPoint.position).normalized;

        // Update camera position based on the current distance
        activeCamera.transform.position = cameraOrbitPoint.position + direction * currentDistance;
    }
}
