using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class PlayerVRInputs : MonoBehaviour
{
    [SerializeField] private PlayerHandAnimator handAnimator = default;

    [SerializeField] private Gun gun = default;
    [SerializeField] private OVRGrabber[] hands = default;

    private InputDevice[] vrControllers = new InputDevice[2]; // Left is 0, Right is 1

    private void Start()
    {
        Debug.Log(gameObject);
        XRSettings.eyeTextureResolutionScale = 2f;
        InitVRInputs();
    }

    private void FixedUpdate()
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
            vrControllers[i].TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
            vrControllers[i].TryGetFeatureValue(CommonUsages.trigger, out float trigger);
            vrControllers[i].TryGetFeatureValue(CommonUsages.grip, out float grip);
            vrControllers[i].TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);
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
            int thumbTarget = 0;
            if(joystickClick)
                thumbTarget = 2;
            else if(joystickTouch)
                thumbTarget = 1;
            handAnimator.AnimateHand(i, trigger, grip, thumbTarget);

            // Shoot
            if(triggerButton
            && gun.grabbable.isGrabbed
            && gun.grabbable.grabbedBy == hands[i])
            {
                gun.Shoot();
            }
        }
    }
}
