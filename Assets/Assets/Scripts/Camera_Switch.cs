using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera firstPersonCam; // Your Main Camera (FPP)
    public Camera thirdPersonCam; // TPP camera

    private bool isFirstPerson = true;

    void Start()
    {
        // Set default state
        firstPersonCam.enabled = true;
        thirdPersonCam.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isFirstPerson = !isFirstPerson;

            firstPersonCam.enabled = isFirstPerson;
            thirdPersonCam.enabled = !isFirstPerson;
        }
    }
}
