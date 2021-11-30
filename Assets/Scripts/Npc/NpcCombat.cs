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
            Vector3 direction = (targetLastSeen - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, direction);
            float closestDist = float.PositiveInfinity;
            for(int i = 0; i < coverGroups.Count; ++i)
            {
                for(int j = 0; j < coverGroups[i].cover.Length; ++j)
                {
                    if(coverGroups[i].cover[j].isOccupied)
                        continue;

                    float maxAngle = Mathf.Cos(90f / 2f * Mathf.Deg2Rad); // Cover needs to face target
                    if(dotProduct >= maxAngle
                    && Vector3.Distance(transform.position, coverGroups[i].cover[j].GetPosition()) < closestDist) // Closest cover
                        newCover = coverGroups[i].cover[j];
                }
            }
        }

        if(newCover != null)
        {
            cover.isOccupied = false;

            newCover.isOccupied = true;
            npc.movement.destination = newCover.GetPosition();
            cover = newCover;
        }
    }
}
