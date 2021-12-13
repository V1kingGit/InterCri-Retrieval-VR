using UnityEngine;
using System.Collections.Generic;
using V1king;

public class NpcCover : MonoBehaviour
{
    private const float BASE_MORALE = 50f;

    [Header("References")]
    [SerializeField] private Npc npc = null;
    [SerializeField] private SphereCollider coverRange = null;
    [Header("Values")]
    [SerializeField] private LayerMask tallObstructions = default;
    [SerializeField] private LayerMask allObstructions = default;
    [SerializeField] private int coverLayer = 0;
    [SerializeField] private int coverDetectionLayer = 0;
    [SerializeField] private float coverChangeInterval = default;

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
            //if(value == DangerStates.Combat && cover == null)
            //    MoveToCover();
            _dangerState = value;
        }
    }

    private List<Npc> nearbyAllies = new List<Npc>();

    private List<CoverGroup> coverGroups = new List<CoverGroup>();
    private CoverGroup.Cover cover;
    private float lastCoverUpdate = float.NegativeInfinity;

    private Vector3 targetLastSeen = Npc.invalidVector;

    private float morale;

    private float preferredFightingRange => MathConversions.ConvertNumberRange(morale, 0f, 100f, npc.spotting.visionRange, 0f);

    private void Awake()
    {
        morale = Random.Range(BASE_MORALE - 50f, BASE_MORALE + 50f);
    }

    private void Update()
    {
        if(dangerState == DangerStates.Combat && Time.time - lastCoverUpdate > coverChangeInterval)
            MoveToCover();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == coverLayer)
            coverGroups.Add(other.GetComponent<CoverGroup>());
        else if(other.gameObject.layer == coverDetectionLayer)
            nearbyAllies.Add(other.GetComponent<NpcCover>().npc);

        morale += 25f;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == coverLayer)
            coverGroups.Remove(other.GetComponent<CoverGroup>());
        else if(other.gameObject.layer == coverDetectionLayer)
            nearbyAllies.Remove(other.GetComponent<NpcCover>().npc);

        morale -= 25f;
    }

    public void UpdateTarget(Vector3 spottedPos, bool toUpdateOthers = false)
    {
        //if(Vector3.Distance(targetLastSeen, spottedPos) < 2f) // Only update if target changed position
        //    return;

        targetLastSeen = spottedPos;
        
        dangerState = DangerStates.Combat;
        //if(cover != null) // If in cover, reevaluate if it still protects
        //    MoveToCover();

        // Share with nearby allies
        if(toUpdateOthers)
        {
            for(int i = 0; i < nearbyAllies.Count; ++i)
                nearbyAllies[i].combat.UpdateTarget(spottedPos, false);
        }
    }

    private void MoveToCover()
    {
        lastCoverUpdate = Time.time;

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
            //float maxAngle = Mathf.Cos(90f / 2f * Mathf.Deg2Rad); // Cover needs to face target
            float bestScore = 0f;
            for(int i = 0; i < coverGroups.Count; ++i)
            {
                for(int j = 0; j < coverGroups[i].cover.Length; ++j)
                {
                    if(coverGroups[i].cover[j].isOccupied)
                        continue;

                    /*Vector3 targetDir = (targetLastSeen - coverGroups[i].cover[j].GetPosition()).normalized;
                    float dotProduct = Vector3.Dot(coverGroups[i].cover[j].direction, targetDir);
                    if(dotProduct >= maxAngle || dotProduct < 0f)
                        continue;*/

                    float proximity = Vector3.Distance(transform.position, coverGroups[i].cover[j].GetPosition());
                    proximity = MathConversions.ConvertNumberRange(proximity, 0f, coverRange.radius, 20f, 0f);
                    if(proximity < 0f)
                        continue;

                    float lineOfFire = 0f; // Includes cover
                    if(Physics.Linecast(coverGroups[i].cover[j].GetPosition(), targetLastSeen, tallObstructions))
                        lineOfFire += 20f;
                    if(Physics.Linecast(coverGroups[i].cover[j].GetPosition(), targetLastSeen, allObstructions))
                        lineOfFire += 20f;

                    float fightingRange = IsInRange(coverGroups[i].cover[j].GetPosition(), targetLastSeen, preferredFightingRange) ? 20f : 0f;

                    float totalScore = proximity + lineOfFire + fightingRange;
                    //float score = dotProduct - V1king.MathConversions.ConvertNumberRange(distance, 0f, npc.spotting.visionRange, 0f, 1f);

                    if(totalScore > bestScore)
                    {
                        newCover = coverGroups[i].cover[j];
                        bestScore = totalScore;
                    }

                    
                    /*if(dotProduct >= maxAngle || dotProduct < 0f || dotProduct > lowestDotProduct) // Best facing direction towards target
                        continue;

                    float distance = Vector3.Distance(transform.position, coverGroups[i].cover[j].GetPosition());
                    if(distance < closestDist) // Closest cover
                    {
                        newCover = coverGroups[i].cover[j];
                        lowestDotProduct = dotProduct;
                        closestDist = distance;
                    }*/
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
        }
    }

    private static bool IsInRange(Vector3 pos, Vector3 targetPos, float fightingRange)
    {
        float fightingRangeMin = fightingRange - 5f;
        float fightingRangeMax = fightingRange + 5f;
        float dist = Vector3.Distance(pos, targetPos);
        return dist >= fightingRangeMin && dist <= fightingRangeMax ? true : false;
    }
}
