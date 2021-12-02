using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRHandAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animation[] handAnimations = null;
    [Header("Values")]
    [SerializeField] private string[] animNames = null;

    private InputDevice[] vrControllers = new InputDevice[2]; // Left is 0, Right is 1

    private void Start()
    {
        InitVRInputs();
    }

    private void Update()
    {
        UpdateVRInputs();
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

    private void UpdateVRInputs()
    {
        for(int i = 0; i < vrControllers.Length; ++i)
        {
            vrControllers[i].TryGetFeatureValue(CommonUsages.grip, out float grip);
            vrControllers[i].TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);
            vrControllers[i].TryGetFeatureValue(CommonUsages.trigger, out float trigger);
            vrControllers[i].TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out bool joystickTouch);
            vrControllers[i].TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out bool joystickClick);

            if(i == 0) // Left hand
            {

            }
            else // Right hand
            {
                //movement = primary2DAxis;
            }
            // Both
            //handAnimations[i].Play(,);
            Animator animator;
        }
    }
}
