using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    protected float health { get; private set; } = 100f;

    [SerializeField] private float damageMultiplier = 1f;

    public virtual void TakeDamage(float damage, Vector3 originPos)
    {
        health -= damage * damageMultiplier;
        if(health <= 0f)
            Destroy(gameObject);
    }
}
