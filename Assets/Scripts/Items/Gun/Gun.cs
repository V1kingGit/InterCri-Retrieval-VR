using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public OVRGrabbable grabbable;
    public Magazine magazine;
    [SerializeField] private Transform model = default;
    [SerializeField] private Transform muzzleEnd = default;
    [SerializeField] private float projectileLength = default;

    [Header("Values")]
    [SerializeField] private bool requiresMagazine = true;
    [SerializeField] private float damage = default;

    [SerializeField] private float recoilGain = default;
    [SerializeField] private float recoilSpeed = default;
    [SerializeField] private float recoilDecaySpeed = default;

    private bool canShoot => requiresMagazine ? magazine && magazine.ammoCount > 0 : true;

    [System.NonSerialized] public float recoil;

    private void Update()
    {
        float angle = Mathf.MoveTowards(model.localEulerAngles.x, recoil, Time.deltaTime * recoilSpeed);
        model.localEulerAngles = new Vector3(angle, 0f, 0f);
        if(recoil > 0f)
            recoil = Mathf.Lerp(recoil, 0f, Time.deltaTime * recoilDecaySpeed);
    }

    public void Shoot()
    {
        if(!canShoot)
            return;

        --magazine.ammoCount;
        recoil += recoilGain;

        Vector3 startPos = muzzleEnd.position;
        startPos.z += projectileLength;
        Projectile projectile = ObjectPooling.singleton.SpawnBullet(startPos, Quaternion.LookRotation(model.forward, Vector3.up));
        projectile.Begin(damage);
    }
}
