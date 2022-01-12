using UnityEngine;

public class ChanceToDisappear : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] private float chance = 0f;

    private void Awake()
    {
        if(Random.value < chance)
            Destroy(gameObject);
    }
}
