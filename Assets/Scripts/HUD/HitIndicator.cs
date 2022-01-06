using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer meshRenderer;
    [Header("Values")]
    [SerializeField] private float lifeTime = 2f;

    [System.NonSerialized] public Vector3 lookAtPos;

    private float time;

    private void LateUpdate()
    {
        transform.LookAt(lookAtPos);
        UpdateFade();
    }

    private void UpdateFade()
    {
        time += Time.deltaTime;
        if(time > lifeTime)
        {
            Destroy(gameObject);
            return;
        }
        Color newColor = meshRenderer.material.color;
        newColor.a = Mathf.Lerp(1f, 0f, time / lifeTime);
        for(int i = 0; i < meshRenderer.materials.Length; ++i)
            meshRenderer.materials[i].color = newColor;
    }
}
