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
        public Vector3 direction;
        [System.NonSerialized] public bool isOccupied;

        public Cover(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
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

        float startPos = -meshRenderer.bounds.extents.x + PADDING + GetCoverStartPos(meshRenderer.bounds.extents.x, spacing, coverAmountX) / 2f;
        for(float x = startPos; x <= meshRenderer.bounds.extents.x - PADDING; x += spacing)
        {
            cover[coverIndex++] = new Cover(transform.position + new Vector3(x, 0f, meshRenderer.bounds.extents.z + ENTITY_WIDTH + coverDistance), -Vector3.forward);
            cover[coverIndex++] = new Cover(transform.position + new Vector3(x, 0f, -meshRenderer.bounds.extents.z - ENTITY_WIDTH - coverDistance), Vector3.forward);
        }

        startPos = -meshRenderer.bounds.extents.z + PADDING + GetCoverStartPos(meshRenderer.bounds.extents.z, spacing, coverAmountZ) / 2f;
        for(float z = startPos; z <= meshRenderer.bounds.extents.z - PADDING; z += spacing)
        {
            cover[coverIndex++] = new Cover(transform.position + new Vector3(meshRenderer.bounds.extents.x + ENTITY_WIDTH + coverDistance, 0f, z), -Vector3.right);
            cover[coverIndex++] = new Cover(transform.position + new Vector3(-meshRenderer.bounds.extents.x - ENTITY_WIDTH - coverDistance, 0f, z), Vector3.right);
        }
    }

    private float GetCoverStartPos(float extents, float spacing, int coverAmount)
    {
        float first = extents * 2f;
        float second = spacing * coverAmount;
        return first - second + 2f;
    }

    private int GetCoverAmount(float size, float spacing)
    {
        if(size >= ENTITY_WIDTH + PADDING)
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
            // Position
            Gizmos.DrawSphere(cover[i].position, 0.25f);

            // HideOffset
            Gizmos.DrawLine(cover[i].position, cover[i].position + cover[i].hideOffset);
            Gizmos.DrawWireSphere(cover[i].position, 0.25f);

            // Direction
            Gizmos.DrawLine(cover[i].GetPosition(), cover[i].GetPosition() + cover[i].direction);
        }
    }
#endif
}
