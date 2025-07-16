using Unity.Mathematics;
using UnityEngine;

public class Mouse_Look : MonoBehaviour
{
    float mouse_Sensitivity = 200f;
    public Transform player_Body;
    float Rotation_X;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouse_Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouse_Sensitivity * Time.deltaTime;
        Rotation_X -= mouseY; // I did used + before but it was inverted, so that is the reason i used - instead of +
        Rotation_X = Mathf.Clamp(Rotation_X, -90f, 90f); // Clamp the rotation to prevent flipping
        transform.localRotation = Quaternion.Euler(Rotation_X, 0f, 0f); // Apply the rotation to the camera
        player_Body.Rotate(Vector3.up * mouseX); // Rotate the player body around the Y-axis based on mouse movement

    }
}
