using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private const float k_MouseSensitivityMultiplier = 0.01f;

    [Header("References")]
    [SerializeField] private Transform playerBody = default;
    [Header("Values")]
    [SerializeField] private bool isComputerControlled = false;
    [SerializeField] private float mouseSensitivity = 100f;

    private float xRotation = 0f;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        UpdateLockInputs();
        if(isComputerControlled)
            UpdateComputerInputs();
        else
            UpdateVRInputs();
    }

    private void UpdateLockInputs()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UpdateComputerInputs()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * k_MouseSensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * k_MouseSensitivityMultiplier;

        if(Cursor.lockState == CursorLockMode.Locked)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private void UpdateVRInputs()
    {
    }
}
