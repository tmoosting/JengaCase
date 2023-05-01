using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform[] towers;
    public float rotationSpeed = 5f;
    public float distance = 10f;
    public float yOffset = 5f;
    public float zoomSpeed = 5f;
    public float minCameraDistance = 5f;
    public float maxCameraDistance = 15f;

    private int currentTowerIndex = 0;
    private Vector3 currentRotation;

    void Start()
    {
        FocusOnTower(currentTowerIndex);
    }

    void Update()
    {
        // Switch towers with left and right arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentTowerIndex--;
            if (currentTowerIndex < 0) currentTowerIndex = towers.Length - 1;
            UpdateCameraPosition();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentTowerIndex++;
            if (currentTowerIndex >= towers.Length) currentTowerIndex = 0;
            UpdateCameraPosition();
        }

        // Orbit around the tower using Mouse 1
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            currentRotation.y += mouseX;
            currentRotation.x -= mouseY;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -80, 80);
        }

        // Zoom in and out using the scroll wheel
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollWheel * zoomSpeed;
        distance = Mathf.Clamp(distance, minCameraDistance, maxCameraDistance);

        UpdateCameraPosition();
    }

    
    private void FocusOnTower(int index)
    {
        if (index < 0 || index >= towers.Length) return;

        Transform tower = towers[index];
        Vector3 targetPosition = tower.position + new Vector3(0, yOffset, 0);
        currentRotation = Quaternion.LookRotation(targetPosition - transform.position).eulerAngles;

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        Vector3 targetPosition = towers[currentTowerIndex].position + new Vector3(0, yOffset, 0);
        transform.position = targetPosition + rotation * direction;
        transform.LookAt(targetPosition);
    }
}