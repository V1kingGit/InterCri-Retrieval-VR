using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("References")]
    public Transform model;
    public OVRGrabbable grabbable;

    [Header("Values")]
    [SerializeField] private int maxAmmo = default;

    [System.NonSerialized] public int ammoCount;

    private void Awake()
    {
        ammoCount = maxAmmo;
    }
}
