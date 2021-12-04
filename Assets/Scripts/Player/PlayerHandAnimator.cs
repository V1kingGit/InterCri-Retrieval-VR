using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class PlayerHandAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator[] animators = null;
    [Header("Values")]
    [SerializeField] private float animSpeed = default;

    private InputDevice[] vrControllers = new InputDevice[2]; // Left is 0, Right is 1

    private float[] triggerCurrent = new float[2];
    private float[] gripCurrent = new float[2];
    private int[] thumbCurrent = new int[2];

    private void Start()
    {
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
            AnimateHand(i, trigger, grip, thumbTarget);
        }
    }

    private void AnimateHand(int index, float triggerTarget, float gripTarget, int thumbTarget)
    {
        if(gripCurrent[index] != gripTarget)
        {
            gripCurrent[index] = Mathf.MoveTowards(gripCurrent[index], gripTarget, Time.deltaTime * animSpeed);
            animators[index].SetFloat("Grip", gripCurrent[index]);
        }

        if(triggerCurrent[index] != triggerTarget)
        {
            triggerCurrent[index] = Mathf.MoveTowards(triggerCurrent[index], triggerTarget, Time.deltaTime * animSpeed);
            animators[index].SetFloat("Trigger", triggerCurrent[index]);
        }

        if(thumbCurrent[index] != thumbTarget)
        {
            thumbCurrent[index] = thumbTarget;
            animators[index].SetInteger("Thumb", thumbCurrent[index]);
        }
    }
}
