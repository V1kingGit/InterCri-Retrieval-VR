using UnityEngine;

public class NpcShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Gun gun = default;

    [Header("Values")]
    [SerializeField] private float maxAcceptableRecoil = default;

    private float nextAcceptableRecoil;

    private bool toShoot;

    private void Update()
    {
        if(toShoot && gun.recoil <= nextAcceptableRecoil)
        {
            nextAcceptableRecoil = Random.Range(0f, maxAcceptableRecoil);
            gun.Shoot();
        }
    }
}
