using UnityEngine;
using System.Collections;

public class PlayerHealth : EntityHealth
{
    [SerializeField] private Material hitIndicatorMat = default;
    [SerializeField] private MeshRenderer[] hitIndicatorBases = default;
    [SerializeField] private GameObject hitIndicatorPrefab = default;

    [SerializeField] private Transform[] lifebarAnchors = default;

    public override void TakeDamage(float damage, Vector3 originPos)
    {
        base.TakeDamage(damage, originPos);

        for(int i = 0; i < hitIndicatorBases.Length; ++i)
        {
            GameObject newIndicator = Instantiate(hitIndicatorPrefab, hitIndicatorBases[i].transform);
            newIndicator.GetComponent<HitIndicator>().lookAtPos = originPos;
        }

        StopAllCoroutines();
        StartCoroutine(DamageFlash());
        StartCoroutine(ChangeLifebarValue());
    }

    private IEnumerator DamageFlash()
    {
        const float duration = 0.5f;
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(Color.red, Color.white, time / duration);
            newColor.a = hitIndicatorBases[0].material.color.a;
            for(int i = 0; i < hitIndicatorBases.Length; ++i)
                hitIndicatorBases[i].material.color = newColor;

            PPEffects.singleton.LerpColorAdjustments((duration - time) / duration);
            yield return null;
        }
    }

    private IEnumerator ChangeLifebarValue()
    {
        const float duration = 0.25f;
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            for(int i = 0; i < lifebarAnchors.Length; ++i)
            {
                Vector3 newScale = lifebarAnchors[i].localScale;
                newScale.x = health / 100f;
                lifebarAnchors[i].localScale = Vector3.Lerp(lifebarAnchors[i].localScale, newScale, time / duration);
            }
            yield return null;
        }
    }
}
