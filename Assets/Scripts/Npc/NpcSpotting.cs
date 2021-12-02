using UnityEngine;
using System.Collections;

// Placed on NPC head
public class NpcSpotting : MonoBehaviour
{
#if UNITY_EDITOR
    private static readonly Color debugSpottedColor = Color.red;
    private static readonly Color debugNoVisionColor = Color.white;
#endif
    private static readonly WaitForSeconds rangeCheckInterval = new WaitForSeconds(3f);
    private static readonly WaitForSeconds fovCheckInterval = new WaitForSeconds(0.2f);
    private static WaitForSeconds reactionTime;
    private static WaitForSeconds peripheralReactionTime; // 100 ms slower than normal

    private const float visualReactionTimeMin = 0.18f;
    private const float visualReactionTimeMax = 0.25f;

    private delegate void ReactionDelegate(int targetIndex);

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;
#endif

    [Header("References")]
    [SerializeField] private Npc npc = null;
    [SerializeField] private Transform[] targets = null;
    [SerializeField] private Transform body = null;

    [Header("Values")]
    [SerializeField] private float[] headSpeeds = new float[System.Enum.GetValues(typeof(NpcCombat.DangerStates)).Length];
    [SerializeField] private float spotPosDuration = default;
    public float visionRange = 85f;
    [SerializeField] private float totalFOV = 115f; // Includes peripheral vision
    [SerializeField] private float maculaFOV = 16f; // Macular vision
    [SerializeField] private LayerMask obstructions = default;

    private enum SpotStage
    {
        None,
        Peripheral,
        Macula
    }
    private SpotStage spotStage;
    private SpotStage[] reactingTo;

    private int currentTarget;
    private bool isInRangeOfTarget;
    private bool[] seesTargets;

    private Vector3 _spotPos = Npc.invalidVector;
    public Vector3 spotPos
    {
        get
        {
            return _spotPos;
        }
        set
        {
            reachedSpotPos = -1f;
            _spotPos = value;
        }
    }
    private float reachedSpotPos = -1f;

    private float suspicion; // 0 to 100

    private void Awake()
    {
        float baseReactionTime = Random.Range(visualReactionTimeMin, visualReactionTimeMax);
        reactionTime = new WaitForSeconds(baseReactionTime);
        peripheralReactionTime = new WaitForSeconds(baseReactionTime + 0.1f);

        reactingTo = new SpotStage[targets.Length];
        seesTargets = new bool[targets.Length];

        StartCoroutine(RangeCheck());
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!drawGizmos)
            return;

        if(isInRangeOfTarget)
        {
            Color gizmosColor = spotStage == SpotStage.Macula ? debugSpottedColor : debugNoVisionColor;
            gizmosColor.a = 1f;
            Gizmos.color = gizmosColor;
            DrawWireArc(transform.position, transform.forward, maculaFOV, visionRange);

            gizmosColor = spotStage == SpotStage.Peripheral ? debugSpottedColor : debugNoVisionColor;
            gizmosColor.a = 0.25f;
            Gizmos.color = gizmosColor;
            DrawWireArc(transform.position, transform.forward, totalFOV, visionRange);
        }
        else
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, visionRange);
        }
    }
#endif

    private void Update()
    {
        suspicion -= Time.deltaTime;
        UpdateSpotting();
        UpdateHeadRotation();
    }

    private void UpdateHeadRotation()
    {
        Quaternion desiredRotation = body.rotation;
        float speedMultiplier = 1f;

        // Look at targets we can see
        for(int i = 0; i < targets.Length; ++i)
        {
            if(!seesTargets[i])
                continue;

            Vector3 direction = targets[i].position - transform.position;
            desiredRotation = DirectionToRotation(direction);
            if(VectorsWithinDeadzone(transform.eulerAngles, desiredRotation.eulerAngles, 2f)) // Don't do aggressive small head movements (would look funny)
                speedMultiplier = 0.05f;
            break;
        }

        // Cant see targets? Look at spotPos
        if(spotPos != Npc.invalidVector)
        {
            if(desiredRotation == body.rotation)
            {
                Vector3 direction = spotPos - transform.position;
                desiredRotation = DirectionToRotation(direction);
            }

            if(VectorsWithinDeadzone(transform.eulerAngles, desiredRotation.eulerAngles, 0.01f))
            {
                if(reachedSpotPos == -1f)
                    reachedSpotPos = Time.time;
                else if(Time.time > reachedSpotPos + spotPosDuration) // Look a bit longer if no other spotted targets - new spotted targets will overwrite this
                {
                    if(suspicion >= 100f && npc.combat.dangerState == NpcCombat.DangerStates.Safe)
                        npc.movement.InvestigateLocation(spotPos);
                    spotPos = Npc.invalidVector;
                }
            }
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * headSpeeds[(int)npc.combat.dangerState] * speedMultiplier);
    }

    private bool VectorsWithinDeadzone(Vector3 v1, Vector3 v2, float deadzone)
    {
        return Mathf.Abs(v1.x - v2.x) < deadzone
            && Mathf.Abs(v1.y - v2.y) < deadzone;
    }

    private static Quaternion DirectionToRotation(Vector3 direction)
    {
        Vector3 desiredEulerAngles = Quaternion.LookRotation(direction).eulerAngles;
        desiredEulerAngles.z = 0f;
        return Quaternion.Euler(desiredEulerAngles);
    }

    private void UpdateSpotting()
    {
        if(spotStage == SpotStage.None)
            return;

        if(++currentTarget >= targets.Length)
            currentTarget = 0;

        if(seesTargets[currentTarget])
        {
            if(!HasLineOfSight(targets[currentTarget].position))
                BeginReactingTo(LoseTarget, currentTarget);
            else
                npc.combat.UpdateTarget(targets[0].position);
        }
        else if(spotStage == SpotStage.Macula)
        {
            if(HasLineOfSight(targets[currentTarget].position))
                BeginReactingTo(SpotTarget, currentTarget);
        }
        else
        {
            if((spotPos == Npc.invalidVector || reachedSpotPos != -1f) && HasLineOfSight(targets[currentTarget].position))
                BeginReactingTo(NoticeTarget, currentTarget);
        }
    }

    private IEnumerator RangeCheck()
    {
        while(true)
        {
            yield return rangeCheckInterval;

            bool newValue = Vector3.Distance(transform.position, targets[0].position) < 85f;
            if(newValue != isInRangeOfTarget)
            {
                if(newValue)
                    StartCoroutine(FOVCheck());
                else
                    StopCoroutine(FOVCheck());
                isInRangeOfTarget = newValue;
            }
        }
    }

    private IEnumerator FOVCheck()
    {
        while(true)
        {
            if(spotStage > SpotStage.None)
                yield return null; // More precise calculations needed from Peripheral -> Macula
            else
                yield return fovCheckInterval; // Peripheral vision doesn't need accuracy

            Vector3 direction = (targets[0].position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, direction);

            float maxMaculaAngle = Mathf.Cos(maculaFOV / 2f * Mathf.Deg2Rad);
            if(dotProduct >= maxMaculaAngle)
            {
                spotStage = SpotStage.Macula;
                continue;
            }

            float maxPeripheralAngle = Mathf.Cos(totalFOV / 2f * Mathf.Deg2Rad);
            if(dotProduct >= maxPeripheralAngle)
            {
                spotStage = SpotStage.Peripheral;
                continue;
            }

            spotStage = SpotStage.None;
        }
    }

    private void BeginReactingTo(ReactionDelegate method, int targetIndex)
    {
        if(!seesTargets[targetIndex] && reactingTo[targetIndex] >= spotStage)
            return;

        StartCoroutine(ReactTo(method, targetIndex));
        reactingTo[targetIndex] = spotStage;
    }

    private IEnumerator ReactTo(ReactionDelegate method, int targetIndex)
    {
        if(spotStage == SpotStage.Macula && npc.combat.dangerState != NpcCombat.DangerStates.Safe)
            yield return reactionTime;
        else
            yield return peripheralReactionTime;

        if(spotStage == SpotStage.Macula)
            method.Invoke(targetIndex);
        else if(HasLineOfSight(targets[targetIndex].position) == !seesTargets[targetIndex]) // If still has / doesn't have line of sight (= fast movements in peripheral aren't detected)
            method.Invoke(targetIndex);
        reactingTo[targetIndex] = SpotStage.None;
    }

    private void SpotTarget(int targetIndex)
    {
        Debug.Log("SpotTarget");

        // Spot enemy position
        npc.combat.UpdateTarget(targets[0].position);

        seesTargets[targetIndex] = true;
    }

    private void NoticeTarget(int targetIndex)
    {
        // Begin looking at enemy
        Debug.Log("NoticeTarget");
        suspicion += 70f;
        spotPos = targets[targetIndex].position;
    }

    private void LoseTarget(int targetIndex)
    {
        Debug.Log("LoseTarget");
        seesTargets[targetIndex] = false;

        // Keep a lookout for it
        spotPos = targets[targetIndex].position;
    }

    // Track player a bit even when no line of sight, so no need to run in update, can use coroutine
    private bool HasLineOfSight(Vector3 position)
    {
#if UNITY_EDITOR
        if(Physics.Linecast(transform.position, position, obstructions))
        {
            Debug.DrawLine(transform.position, position, Color.white);
            return false;
        }
        Debug.DrawLine(transform.position, position, Color.red);
        return true;
#else
        if(Physics.Linecast(transform.position, position, obstructions))
            return false;
        return true;
#endif
    }

#if UNITY_EDITOR
    private static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
    {
        float startAngles = GetAnglesFromDir(position, dir);
        float stepAngles = anglesRange / maxSteps;
        float firstAngle = startAngles - anglesRange / 2f;

        Vector3 posA = position;
        float angle = firstAngle;
        for(int i = 0; i <= maxSteps; ++i)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 posB = position;
            posB += new Vector3(radius * Mathf.Cos(rad), 0f, radius * Mathf.Sin(rad));

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Gizmos.DrawLine(posA, position);

        /*posA = position;
        angle = startAngle;
        for(int i = 0; i <= maxSteps; ++i)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 posB = position;
            posB += new Vector3(0f, radius, 0f);

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Gizmos.DrawLine(posA, position);*/
    }

    private static float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        Vector3 forwardLimitPos = position + dir;
        float srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);
        return srcAngles;
    }
#endif
}
