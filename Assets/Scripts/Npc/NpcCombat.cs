using UnityEngine;
using System.Collections.Generic;

public class NpcCombat : MonoBehaviour
{
    /*public class Squad
    {
        private List<Npc> npcs = new List<Npc>();
    }
    private Squad squad;*/

    [Header("References")]
    [SerializeField] private Npc npc = null;

    public enum DangerStates
    {
        Safe, // Doesn't expect enemies
        Cautious, // Keep an eye out
        Combat // Knows there is an enemy that could be capable of shooting us
    }
    private DangerStates _dangerState;
    public DangerStates dangerState
    {
        get
        {
            return _dangerState;
        }
        set
        {
            if(value == DangerStates.Combat && cover == null)
                MoveToCover();
            _dangerState = value;
        }
    }

    private List<CoverGroup> coverGroups = new List<CoverGroup>();
    private CoverGroup.Cover cover;

    private Vector3 targetLastSeen = Npc.invalidVector;

    /*private void Update()
    {
        // Debug
        if(targetLastSeen == Npc.invalidVector)
            return;

        float maxAngle = Mathf.Cos(90f / 2f * Mathf.Deg2Rad); // Cover needs to face target
        for(int i = 0; i < coverGroups.Count; ++i)
        {
            for(int j = 0; j < coverGroups[i].cover.Length; ++j)
            {
                Vector3 targetDir = (targetLastSeen - coverGroups[i].cover[j].GetPosition()).normalized;
                float dotProduct = Vector3.Dot(coverGroups[i].cover[j].direction, targetDir);
                Debug.Log(maxAngle + " | " + dotProduct);
            }
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        coverGroups.Add(other.GetComponent<CoverGroup>());
    }

    private void OnTriggerExit(Collider other)
    {
        coverGroups.Remove(other.GetComponent<CoverGroup>());
    }

    public void UpdateTarget(Vector3 spottedPos)
    {
        targetLastSeen = spottedPos;
        dangerState = DangerStates.Combat;
        if(cover != null) // If in cover, reevaluate if it still protects
            MoveToCover();
    }

    private void MoveToCover()
    {
        if(cover != null) // Already has cover
        {
            if(targetLastSeen == Npc.invalidVector)
                return; // Noone specific to protect from, current cover will suit fine

            // Reevaluate whether cover protects from target
            Vector3 direction = (targetLastSeen - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, direction);
            float maxAngle = Mathf.Cos(90f / 2f * Mathf.Deg2Rad);
            if(dotProduct >= maxAngle)
                return; // Still valid cover from target
        }

        CoverGroup.Cover newCover = null;
        if(targetLastSeen == Npc.invalidVector) // No known target to hide from
        {
            float closestDist = float.PositiveInfinity;
            for(int i = 0; i < coverGroups.Count; ++i)
            {
                for(int j = 0; j < coverGroups[i].cover.Length; ++j)
                {
                    if(coverGroups[i].cover[j].isOccupied)
                        continue;

                    if(Vector3.Distance(transform.position, coverGroups[i].cover[j].GetPosition()) < closestDist) // Closest cover
                        newCover = coverGroups[i].cover[j];
                }
            }
        }
        else // Specifically use cover that protects from target
        {
            float maxAngle = Mathf.Cos(90f / 2f * Mathf.Deg2Rad); // Cover needs to face target
            float lowestDotProduct = 1f;
            float closestDist = float.PositiveInfinity;
            for(int i = 0; i < coverGroups.Count; ++i)
            {
                for(int j = 0; j < coverGroups[i].cover.Length; ++j)
                {
                    if(coverGroups[i].cover[j].isOccupied)
                        continue;

                    Vector3 targetDir = (targetLastSeen - coverGroups[i].cover[j].GetPosition()).normalized;
                    float dotProduct = Vector3.Dot(coverGroups[i].cover[j].direction, targetDir);
                    if(dotProduct >= maxAngle || dotProduct < 0f || dotProduct > lowestDotProduct) // Best facing direction towards target
                        continue;

                    float distance = Vector3.Distance(transform.position, coverGroups[i].cover[j].GetPosition());
                    if(distance < closestDist) // Closest cover
                    {
                        newCover = coverGroups[i].cover[j];
                        lowestDotProduct = dotProduct;
                        closestDist = distance;
                    }
                }
            }
        }

        if(newCover != null)
        {
            if(cover != null)
                cover.isOccupied = false;

            newCover.isOccupied = true;
            npc.movement.destination = newCover.GetPosition();
            cover = newCover;

            Vector3 targetDir = (targetLastSeen - cover.GetPosition()).normalized;
            float dotProduct = Vector3.Dot(cover.direction, targetDir);
            float distance = Vector3.Distance(transform.position, cover.GetPosition());
            float maxAngle = Mathf.Cos(90f / 2f * Mathf.Deg2Rad); // Cover needs to face target
            Debug.Log(maxAngle + " | " + dotProduct + " | " + distance);
        }
    }
}
