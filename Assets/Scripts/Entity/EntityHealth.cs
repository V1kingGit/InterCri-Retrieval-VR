using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    private float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0f)
            Destroy(gameObject);
    }
}
