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
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        public void Initialize()
        {
            initialPosition = camera.transform.position;
            initialRotation = camera.transform.rotation;
        }

        public void ResetPositionAndRotation()
        {
            camera.transform.position = initialPosition;
            camera.transform.rotation = initialRotation;
        }
    }

    public GameObject cameraOrbitPoint;
    public float rotationSpeed = 5f; // Adjust the rotation speed as needed

    private int activeCameraIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Store initial position and rotation for each camera and disable all cameras except the first one
        for (int i = 0; i < cameraData.Length; i++)
        {
            cameraData[i].Initialize();
            cameraData[i].camera.SetActive(i == activeCameraIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameras();
        }

        // Check for numeric key presses to switch cameras
        for (int i = 0; i < cameraData.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetActiveCamera(i);
            }
        }

        if (cameraData[activeCameraIndex].canOrbit)
        {
            OrbitCamera();
        }

        ResetCameraPosition();
    }

    void ToggleCameras()
    {
        // Toggle to the next camera
        SetActiveCamera((activeCameraIndex + 1) % cameraData.Length);
    }

    void SetActiveCamera(int cameraIndex)
    {
        // Reset the current active camera position and rotation before changing
        cameraData[activeCameraIndex].ResetPositionAndRotation();
        // Deactivate the current camera
        cameraData[activeCameraIndex].camera.SetActive(false);
        // Update active camera index
        activeCameraIndex = cameraIndex;
        // Activate the new active camera
        cameraData[activeCameraIndex].camera.SetActive(true);
    }

    void OrbitCamera()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotation = new Vector3(-mouseY, mouseX, 0f) * rotationSpeed;

            // Rotate camera around the orbit point
            cameraData[activeCameraIndex].camera.transform.RotateAround(cameraOrbitPoint.transform.position, Vector3.up, rotation.y); // Rotate around Y-axis
            cameraData[activeCameraIndex].camera.transform.RotateAround(cameraOrbitPoint.transform.position, cameraData[activeCameraIndex].camera.transform.right, rotation.x); // Rotate around X-axis
        }
    }

    void ResetCameraPosition()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            // Reset the camera to its initial position and rotation
            cameraData[activeCameraIndex].ResetPositionAndRotation();
        }
    }
}
