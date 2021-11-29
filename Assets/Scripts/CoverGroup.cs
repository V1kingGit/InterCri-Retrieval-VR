using UnityEngine;

public class CoverGroup : MonoBehaviour
{
    private const float ENTITY_WIDTH = 1f;
    private const float PADDING = ENTITY_WIDTH / 2f; // Padding so entities dont stick out half their bodies on the edges of an object

    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer = default; // MeshFilter.mesh alternatively (interaction with scale?)

    [Header("Values")]
    [Header("Auto Generation")]
    [SerializeField] private bool autoGenerateCover = true;
    [Tooltip("How many extra meters between the edge of every entity in cover.")]
    [SerializeField] private float coverSpacing = 1f;
    [Tooltip("The distance between the object and entities taking cover.")]
    [SerializeField] private float coverDistance = 0.25f;

    [System.Serializable]
    public class Cover
    {
        public Vector3 position;
        public Vector3 hideOffset;
        [System.NonSerialized] public bool isOccupied;

        public Cover(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 GetPosition()
        {
            return position + hideOffset;
        }
    }
    public Cover[] cover;

    private void Awake()
    {
        GenerateCover();
    }

    // ToDo: Tall cover, check y bounds and set offset accordingly, with only 2 cover spots possible on each side
    private void GenerateCover()
    {
        if(!autoGenerateCover)
            return;

        float spacing = ENTITY_WIDTH * 2f + coverSpacing;

        // Get amount of cover
        int coverAmountX = GetCoverAmount(meshRenderer.bounds.size.x, spacing);
        int coverAmountZ = GetCoverAmount(meshRenderer.bounds.size.z, spacing);
        cover = new Cover[coverAmountX * 2 + coverAmountZ * 2];

        // Generate cover along X axis
        int coverIndex = 0;
        float offsetX = -meshRenderer.bounds.extents.x;

        /*for(int i = 0; i < coverAmountX; ++i)
        {
            cover[coverIndex++] = new Cover(transform.position + new Vector3(offsetX, 0f, meshRenderer.bounds.extents.z + coverDistance));
            offsetX += 
        }*/

        //float startPos = -meshRenderer.bounds.extents.x + PADDING + spacing * coverAmountX;
        for(float x = -meshRenderer.bounds.extents.x; x <= meshRenderer.bounds.extents.x - PADDING; x += spacing)
        {
            cover[coverIndex++] = new Cover(transform.position + new Vector3(x, 0f, meshRenderer.bounds.extents.z + ENTITY_WIDTH + coverDistance));
        }
    }

    private int GetCoverAmount(float size, float spacing)
    {
        if(size >= ENTITY_WIDTH)
            return (int)(1 + (size - PADDING * 2f) / spacing);
        return 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        GenerateCover();

        Gizmos.color = Color.white;
        for(int i = 0; i < cover.Length; ++i)
        {
            Gizmos.DrawSphere(cover[i].position, 0.25f);
            Gizmos.DrawLine(cover[i].position, cover[i].position + cover[i].hideOffset);
            Gizmos.DrawWireSphere(cover[i].position, 0.25f);
        }
    }
#endif
}
