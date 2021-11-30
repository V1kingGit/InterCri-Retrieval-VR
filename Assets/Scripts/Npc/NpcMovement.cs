using UnityEngine;
using UnityEngine.AI;
using V1king;

public class NpcMovement : MonoBehaviour
{
    private const float DESTINATION_PROXIMITY = 0.1f;

    [Header("References")]
    [SerializeField] private NavMeshAgent navMeshAgent = null;
    [SerializeField] private CharacterController charController = null;

    [Header("Values")]
    [SerializeField] private float rotationSpeed = default;
    public bool reachedDestination { private set; get; }

    private float lastPathfind = float.NegativeInfinity;

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
            lastPathfind = float.NegativeInfinity;
            reachedDestination = false;
            _destination = value;
        }
    }

    private void Awake()
    {
        startPos = transform.position;
        destination = transform.position;
        UpdatePathfind();
    }

    private void Update()
    {
        navMeshAgent.transform.localPosition = Vector3.zero;
        UpdatePathfind();

        UpdateMovement();
        UpdateRotation();

        if(navMeshAgent.remainingDistance <= DESTINATION_PROXIMITY)
            OnDestinationReached();
    }

    public void ReturnToStationedPos()
    {
        destination = startPos;
    }

    private void OnDestinationReached()
    {
        reachedDestination = true;
    }

    private void UpdatePathfind()
    {
        // If target, if static point dont do this
        if(Time.time > lastPathfind + navMeshAgent.remainingDistance / 10f)
            navMeshAgent.SetDestination(destination);
    }

    private void UpdateMovement()
    {
        Vector3 desiredMovement = navMeshAgent.desiredVelocity;
        desiredMovement.y = 0f;
        charController.Move(desiredMovement * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        if(navMeshAgent.remainingDistance <= DESTINATION_PROXIMITY * 2f)
            return;

        Vector3 direction = navMeshAgent.steeringTarget - transform.position;
        float angle = VectorConversions.GetAngleFromVector(direction);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.SmoothStep(transform.eulerAngles.y, angle, Time.deltaTime * rotationSpeed), transform.eulerAngles.z);
    }
}