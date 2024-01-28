using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float sensitivity = 2f;

    void Update()
    {
        // Movement
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalMovement, 0f, verticalMovement).normalized;
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime);

        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX * sensitivity);

        float currentRotationX = transform.rotation.eulerAngles.x;
        float newRotationX = currentRotationX - mouseY * sensitivity;

        if (newRotationX > 90f && newRotationX < 270f)
        {
            newRotationX = (newRotationX > 180f) ? 270f : 90f;
        }

        transform.rotation = Quaternion.Euler(newRotationX, transform.rotation.eulerAngles.y, 0f);
    }
}
