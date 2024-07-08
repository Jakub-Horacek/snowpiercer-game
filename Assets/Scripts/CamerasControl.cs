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

    private int activeCameraIndex = 0;
    private bool isOrbiting = false; // Flag to track if orbiting is active

    private Quaternion initialOrbitPointRotation; // Store initial rotation of cameraOrbitPoint

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
                ResetCameraOrbitPointRotation();
                isOrbiting = false;
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
        ResetCameraOrbitPointRotation();

        // Deactivate the current camera
        cameraData[activeCameraIndex].camera.SetActive(false);
        // Update active camera index
        activeCameraIndex = cameraIndex;
        // Activate the new active camera
        cameraData[activeCameraIndex].camera.SetActive(true);
    }

    void OrbitCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 rotation = new Vector3(-mouseY, mouseX, 0f) * rotationSpeed;

        // Rotate camera orbit point (parent of all cameras)
        cameraOrbitPoint.Rotate(Vector3.up, rotation.y, Space.World); // Rotate around Y-axis
        cameraOrbitPoint.Rotate(cameraOrbitPoint.right, rotation.x, Space.World); // Rotate around X-axis
    }

    void ResetCameraOrbitPointRotation()
    {
        cameraOrbitPoint.rotation = initialOrbitPointRotation; // Reset rotation to initial rotation
    }
}
