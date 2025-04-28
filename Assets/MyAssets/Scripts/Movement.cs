using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject ground;
    private float sensitivity = 2f; // Sensitivity of the camera movement in relation to the mouse position
    private float panSpeed; // Speed of panning
    private Vector3 lastPanPosition; // Last position of the mouse
    private Camera cam; // Reference to the camera

    void Start()
    {
        cam = GetComponent<Camera>();
        SetPanspeed();
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            transform.Translate(new Vector3(0, -Input.mouseScrollDelta.y, 0));
            SetPanspeed();
        }
        // Check if the right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            // Store the initial mouse position
            lastPanPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        // If the right mouse button is held down
        if (Input.GetMouseButton(1))
        {
            // Calculate the offset from the last mouse position
            Vector3 offset = lastPanPosition - cam.ScreenToViewportPoint(Input.mousePosition);

            // Update the last mouse position
            lastPanPosition = cam.ScreenToViewportPoint(Input.mousePosition);

            // Calculate the movement based on the offset
            Vector3 move = new(offset.x * panSpeed, 0, offset.y * panSpeed);

            // Move the camera
            transform.Translate(move, Space.World);
            Vector3 location = transform.position;

            location.x = Mathf.Clamp(location.x, 5, ground.transform.localScale.x - 5);
            location.z = Mathf.Clamp(location.z, -5, ground.transform.localScale.z - 5);
            transform.position = location;
        }
    }

    private void SetPanspeed()
    {
        panSpeed = transform.position.y * sensitivity;
    }
}
