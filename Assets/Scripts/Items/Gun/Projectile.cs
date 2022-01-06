using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float speed = default;
    [SerializeField] private LayerMask collisionLayers = default;

    [System.NonSerialized] public float damage;

    private Vector3 originPos;
    private bool hasCollided;

    private void Update()
    {
        if(!hasCollided)
            Advance();
    }

    public void Begin(float damage)
    {
        this.damage = damage;

        originPos = transform.position;
        hasCollided = false;
        StopAllCoroutines();
        StartCoroutine(FadeOut(10f));
    }

    private void Advance()
    {
        float movementDist = speed * Time.deltaTime;
        Vector3 movement = transform.forward * movementDist;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, movement, out hit, movementDist, collisionLayers))
        {
            PlayerHealth hitPlayer = hit.transform.GetComponentInParent<PlayerHealth>();
            if(hitPlayer)
                hitPlayer.TakeDamage(damage, originPos);
            else
            {
                EntityHealth hitEntity = hit.transform.GetComponentInParent<EntityHealth>();
                if(hitEntity)
                    hitEntity.TakeDamage(damage, originPos);
            }

            hasCollided = true;
            transform.position = hit.point;
            StopAllCoroutines();
            ObjectPooling.singleton.Despawn(gameObject);
        }

        if(!hasCollided)
            transform.position += movement;
    }

    private IEnumerator FadeOut(float duration)
    {
        yield return new WaitForSeconds(duration);
        /*float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(tracer.endColor.a, 0f, time / duration);
            tracer.endColor = new Color(tracer.endColor.r, tracer.endColor.g, tracer.endColor.b, alpha);
            yield return null;
        }*/
        ObjectPooling.singleton.Despawn(gameObject);
    }
}
