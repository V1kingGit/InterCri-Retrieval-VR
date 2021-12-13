using UnityEngine;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling singleton;

    public GameObject prefabProjectile;
    private List<Projectile> projectiles = new List<Projectile>();

    private void Awake()
    {
        singleton = this;
    }

    public Projectile SpawnBullet(Vector3 position, Quaternion rotation)
    {
        Projectile bullet;

        // Retrieve from pool
        for(int i = 0; i < projectiles.Count; ++i)
        {
            if(Retrieve(projectiles[i].gameObject, position, rotation))
            {
                bullet = projectiles[i];
                return bullet;
            }
        }

        // Create new
        bullet = Instantiate(prefabProjectile, position, rotation, null).GetComponent<Projectile>();
        projectiles.Add(bullet);
        return bullet;
    }

    public bool Retrieve(GameObject obj, Vector3 position, Quaternion rotation)
    {
        if(obj.activeSelf)
            return false;
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return true;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.localPosition = Vector3.zero;
    }
}
