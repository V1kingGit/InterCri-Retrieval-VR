using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NpcMovement : MonoBehaviour
{
    private const float DESTINATION_PROXIMITY = 0.1f;

    private static readonly WaitForSeconds investigationCheckInterval = new WaitForSeconds(2f);

    [Header("References")]
    [SerializeField] private Npc npc = null;
    [SerializeField] private NavMeshAgent navMeshAgent = null;
    [SerializeField] private CharacterController charController = null;
    [SerializeField] private Transform head = null;

    [Header("Values")]
    [SerializeField] private float rotationSpeed = default;
    public bool reachedDestination { private set; get; }

    //private float lastPathfind = float.NegativeInfinity;

    private Vector3 startPos;

    private Vector3 _destination;
    public Vector3 destination
    {
        get
        {
            return _destination;
        }
        set
        {
            //lastPathfind = float.NegativeInfinity;
            reachedDestination = false;
            _destination = value;
            navMeshAgent.SetDestination(destination);
        }
    }

    //private Vector3 investigationPos;
    private float investigationStartTime;
    private int investigationWanders;
    private int desiredInvestigationWanders;

    private void Awake()
    {
        startPos = transform.position;
        destination = transform.position;
        //UpdatePathfind();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(navMeshAgent.pathEndPosition, 0.25f);
    }
#endif

    private void Update()
    {
        navMeshAgent.transform.localPosition = Vector3.zero;
        //UpdatePathfind();

        UpdateMovement();
        UpdateRotation();

        if(navMeshAgent.remainingDistance <= DESTINATION_PROXIMITY)
            OnDestinationReached();
    }

    private void ReturnToStationedPos()
    {
        destination = startPos;
    }

    private void OnDestinationReached()
    {
        if(navMeshAgent.pathPending)
            return;

        reachedDestination = true;
    }

    public void InvestigateLocation(Vector3 location)
    {
        if(!NavMesh.SamplePosition(location, out NavMeshHit hit, 100f, navMeshAgent.areaMask))
            return;

        npc.combat.dangerState = NpcCombat.DangerStates.Cautious;
        destination = hit.position;
        //investigationPos = hit.position;
        investigationStartTime = Time.time;
        desiredInvestigationWanders = Random.Range(1, 9);
        StartCoroutine(CheckAroundInvestigation());
    }

    private IEnumerator CheckAroundInvestigation()
    {
        while(investigationWanders < desiredInvestigationWanders)
        {
            yield return investigationCheckInterval;

            float radius = Time.time - investigationStartTime;
            Vector3 investigatePos = GetRandomPos(destination, radius, navMeshAgent.areaMask);
            if(investigatePos == Npc.invalidVector)
                continue;
            if(reachedDestination)
            {
                destination = investigatePos;
                ++investigationWanders;
            }
            else
                npc.spotting.spotPos = investigatePos;
        }

        ReturnToStationedPos();
    }

    private static Vector3 GetRandomPos(Vector3 fromPos, float radius, int areaMask)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir.y /= 2f;
        if(NavMesh.SamplePosition(fromPos + randomDir, out NavMeshHit hit, 100f, areaMask))
            return hit.position;
        return Npc.invalidVector;
    }

    /*private void UpdatePathfind()
    {
        // If target, if static point dont do this
        if(Time.time > lastPathfind + navMeshAgent.remainingDistance / 10f)
        {
            navMeshAgent.SetDestination(destination);
            lastPathfind = Time.time;
        }
    }*/

    private void UpdateMovement()
    {
        if(reachedDestination)
            return;

        Vector3 desiredMovement = navMeshAgent.desiredVelocity;
        desiredMovement.y = 0f;
        charController.Move(desiredMovement * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        Quaternion desiredRotation = transform.rotation;
        if(navMeshAgent.remainingDistance > DESTINATION_PROXIMITY * 1.1f)
        {
            Vector3 direction = navMeshAgent.steeringTarget - transform.position;
            desiredRotation = DirectionToBodyRotation(direction);
        }

        // Follow excessive head rotation if it goes out of constraints
        float headAngle = head.localEulerAngles.y;
        if(headAngle > 180f)
            headAngle -= 360f;
        if(headAngle > 80f)
            desiredRotation.eulerAngles += new Vector3(0f, headAngle - 80f, 0f);
        else if(headAngle < -80f)
            desiredRotation.eulerAngles -= new Vector3(0f, Mathf.Abs(headAngle) - 80f, 0f);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

    public static Quaternion DirectionToBodyRotation(Vector3 direction)
    {
        Vector3 desiredEulerAngles = Quaternion.LookRotation(direction).eulerAngles;
        desiredEulerAngles.x = 0f;
        desiredEulerAngles.z = 0f;
        return Quaternion.Euler(desiredEulerAngles);
    }
}