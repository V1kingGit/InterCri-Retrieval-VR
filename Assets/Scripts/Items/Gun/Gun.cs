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
    [SerializeField] private bool isNpcGun = default;

    [SerializeField] private bool requiresMagazine = true;
    [SerializeField] private float damage = default;

    [SerializeField] private float recoilGain = default;
    [SerializeField] private float recoilSpeed = default;
    [SerializeField] private float recoilDecaySpeed = default;

    public bool canShoot => requiresMagazine ? magazine && magazine.ammoCount > 0 : true;

    [System.NonSerialized] public float recoil;

    private void Update()
    {
        if(!isNpcGun)
        {
            float currentAngle = model.localEulerAngles.x;
            if(currentAngle > 180f)
                currentAngle -= 360f;
            float angle = Mathf.MoveTowards(currentAngle, -recoil, Time.deltaTime * recoilSpeed);
            model.localEulerAngles = new Vector3(angle, 0f, 0f);
        }

        if(recoil > 0f)
            recoil = Mathf.Lerp(recoil, 0f, Time.deltaTime * recoilDecaySpeed);
    }

    public void Shoot(Quaternion direction = default, bool customDir = false)
    {
        if(magazine)
            --magazine.ammoCount;

        recoil += recoilGain;

        if(!customDir)
            direction = Quaternion.LookRotation(model.forward, Vector3.up);
        Projectile projectile = ObjectPooling.singleton.SpawnBullet(GetShootingPos(), direction);
        projectile.Begin(damage);
    }

    public Vector3 GetShootingPos()
    {
        Vector3 startPos = muzzleEnd.position;
        startPos += transform.forward * projectileLength;
        return startPos;
    }
}
