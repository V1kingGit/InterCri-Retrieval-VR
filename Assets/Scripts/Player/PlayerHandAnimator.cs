using UnityEngine;

public class PlayerHandAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator[] animators = null;
    [Header("Values")]
    [SerializeField] private float animSpeed = default;

    private float[] triggerCurrent = new float[2];
    private float[] gripCurrent = new float[2];
    private int[] thumbCurrent = new int[2];

    public void AnimateHand(int index, float triggerTarget, float gripTarget, int thumbTarget)
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
