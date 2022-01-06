using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    private float health = 100f;

    public virtual void TakeDamage(float damage, Vector3 originPos)
    {
        health -= damage;
        if(health <= 0f)
            Destroy(gameObject);
    }
}
