using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
//using Unity.XR.Oculus;

public class PlayerInputs : MonoBehaviour
{
    public static Vector2 movement;

    private InputDevice[] vrControllers = new InputDevice[2]; // Left is 0, Right is 1

    private void Start()
    {
        InitVRInputs();
    }

    private void Update()
    {
        UpdateComputerInputs();
        //UpdateVRInputs();
    }

    private void InitVRInputs()
    {
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);

        for(int i = 0; i < inputDevices.Count; ++i)
        {
            if(inputDevices[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
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
            vrControllers[i].TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton);
            vrControllers[i].TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouch);

            if(i == 0) // Left hand
            {
                Debug.Log($"Trigger: {primaryTouch} | TriggerButton: {primaryButton}");
            }
            else // Right hand
            {
                movement = primary2DAxis;
            }
            // Both
        }

        //Debug.Log($"PrimaryIndexTrigger:{OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch)} | SecondaryIndexTrigger: {OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.LTouch)}");
       
        //bool test = OculusUsages.indexTouch;
        //if(OculusUsages.indexTouch)
        //Debug.Log(OculusUsages.indexTouch + " | " + OculusUsages.thumbrest + " | " + OculusUsages.thumbTouch);
    }
}
