using UnityEngine;

public class Rotate3DModel_YAxis : MonoBehaviour
{
    public float rotationSpeed = 50f; // Tốc độ xoay
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        // Nhấn giữ chuột trái để xoay
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Khi giữ chuột, chỉ xoay quanh trục Y
        if (isDragging)
        {
            float deltaX = Input.mousePosition.x - lastMousePosition.x;
            transform.Rotate(Vector3.up, -deltaX * rotationSpeed * Time.deltaTime, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }
}
