using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class PlayerInputs : MonoBehaviour
{
    public static Vector2 movement;

    private InputDevice[] vrControllers = new InputDevice[2]; // Left is 0, Right is 1
    private InputDevice headset;

    private void Awake()
    {
        InitVRInputs();
    }

    private void Update()
    {
        UpdateComputerInputs();
        UpdateVRInputs();
    }

    private void InitVRInputs()
    {
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);

        for(int i = 0; i < inputDevices.Count; ++i)
        {
            if(inputDevices[i].characteristics.HasFlag(InputDeviceCharacteristics.TrackingReference))
                headset = inputDevices[i];
            else if(inputDevices[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
                vrControllers[0] = inputDevices[i];
            else if(inputDevices[i].characteristics.HasFlag(InputDeviceCharacteristics.Right))
                vrControllers[1] = inputDevices[i];
        }
    }

    private void UpdateComputerInputs()
    {
        movement = Vector3.zero;

        if(Input.GetKey(KeyCode.W))
            movement.y += 1f;
        else if(Input.GetKey(KeyCode.S))
            movement.y -= 1f;

        if(Input.GetKey(KeyCode.A))
            movement.x -= 1f;
        else if(Input.GetKey(KeyCode.D))
            movement.x += 1f;

        movement = movement.normalized;
    }

    private void UpdateVRInputs()
    {
        for(int i = 0; i < vrControllers.Length; ++i)
        {
            vrControllers[i].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxis);
            vrControllers[i].TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);

            if(i == 0) // Left hand
            {
            }
            else // Right hand
            {
                movement = primary2DAxis;
            }
            // Both

        }
    }
}
