using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("References")]
    public Transform model;
    public OVRGrabbable grabbable;
    public Rigidbody rb;
    public Collider coll;

    [Header("Values")]
    [SerializeField] private int maxAmmo = default;

    [System.NonSerialized] public int ammoCount;

    private void Awake()
    {
        SetAmmoToMax();
    }

    public void SetAmmoToMax()
    {
        ammoCount = maxAmmo;
    }
}
