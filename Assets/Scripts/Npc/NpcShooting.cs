using UnityEngine;

public class NpcShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Npc npc = default;
    [SerializeField] private Gun gun = default;

    [Header("Values")]
    [SerializeField] private float maxAcceptableRecoil = default;

    private float nextAcceptableRecoil;

    private bool isReloading;

    private void Update()
    {
        if(isReloading)
            return;
        if(gun.magazine.ammoCount <= 0)
        {
            Invoke(nameof(Reload), 1.3f);
            isReloading = true;
            return;
        }
        if(npc.spotting.visibleTargets == 0)
            return;
        if(!gun.canShoot)
            return;
        if(gun.recoil > nextAcceptableRecoil)
            return;

        int targetIndex = Random.Range(0, npc.spotting.visibleTargets);
        int visibleCounter = 0;
        for(int i = 0; i < npc.spotting.targets.Length; ++i)
        {
            if(!npc.spotting.seesTargets[i])
                continue;

            if(visibleCounter++ == targetIndex)
            {
                nextAcceptableRecoil = Random.Range(0.01f, maxAcceptableRecoil);
                Vector3 dir = npc.spotting.targets[i].position - gun.GetShootingPos();
                gun.Shoot(Quaternion.LookRotation(dir), true);
                return;
            }
        }
    }

    private void Reload()
    {
        gun.magazine.SetAmmoToMax();
        isReloading = false;
    }
}
