using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    [System.NonSerialized] public MeshRenderer meshRenderer;
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
        Color newColor = meshRenderer.material.color;
        newColor.a = Mathf.Lerp(1f, 0f, time / lifeTime);
        meshRenderer.material.color = newColor;
    }
}
