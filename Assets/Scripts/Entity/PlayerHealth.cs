using UnityEngine;
using System.Collections;

public class PlayerHealth : EntityHealth
{
    [SerializeField] private MeshRenderer hitIndicatorBase = default;
    [SerializeField] private GameObject hitIndicator = default;

    public override void TakeDamage(float damage, Vector3 originPos)
    {
        base.TakeDamage(damage, originPos);

        GameObject newIndicator = Instantiate(hitIndicator, hitIndicatorBase.transform);
        newIndicator.GetComponent<HitIndicator>().lookAtPos = originPos;

        StopCoroutine(DamageFlash());
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        const float duration = 0.5f;
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            hitIndicatorBase.material.color = Color.Lerp(Color.red, Color.white, time / duration);
            yield return null;
        }
    }
}
