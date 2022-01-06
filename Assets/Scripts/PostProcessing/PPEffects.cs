using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PPEffects : MonoBehaviour
{
    public static PPEffects singleton;

    [SerializeField] private VolumeProfile ppVolume = null;

    private ColorAdjustments colorAdjustments;
    private float origPostExposure;
    private float origContrast;
    private Color origColorFilter;
    private float origSaturation;

    private void Awake()
    {
        singleton = this;
        ppVolume.TryGet(out colorAdjustments);
        origPostExposure = colorAdjustments.postExposure.value;
        origContrast = colorAdjustments.contrast.value;
        origColorFilter = colorAdjustments.colorFilter.value;
        origSaturation = colorAdjustments.saturation.value;
        LerpColorAdjustments(0f);
    }

    public void LerpColorAdjustments(float interp)
    {
        colorAdjustments.postExposure.Override(Mathf.Lerp(0f, origPostExposure, interp));
        colorAdjustments.contrast.Override(Mathf.Lerp(0f, origContrast, interp));
        colorAdjustments.colorFilter.Override(Color.Lerp(Color.white, origColorFilter, interp));
        colorAdjustments.saturation.Override(Mathf.Lerp(0f, origSaturation, interp));
    }
}