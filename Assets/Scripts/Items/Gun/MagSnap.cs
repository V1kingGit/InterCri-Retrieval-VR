using UnityEngine;

public class MagSnap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform destination = default;

    private Magazine currentMagazine;
    private float distToDestination;

    private void Awake()
    {
        distToDestination = Vector3.Distance(transform.position, destination.position);
    }

    private void LateUpdate()
    {
        if(!currentMagazine)
            return;

        Vector3 relativeMagPos = destination.InverseTransformPoint(currentMagazine.transform.position);
        float interp = relativeMagPos.z / distToDestination;
        //float interp = Vector3.Distance(currentMagazine.transform.position, destination.position) / distToDestination;
        if(interp < -2f - 1f) // -1f buffer
        {
            StopSnapping();
            return;
        }
        if(!currentMagazine.grabbable.isGrabbed)
        {
            if(interp < -0.05f)
            {
                StopSnapping();
                return;
            }
            currentMagazine.transform.SetParent(destination);
        }
        currentMagazine.model.position = Vector3.Lerp(destination.position, transform.position, interp);
        currentMagazine.model.eulerAngles = transform.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentMagazine)
            return;
        if(!other.attachedRigidbody)
            return;

        Magazine magazine = other.attachedRigidbody.GetComponent<Magazine>();
        if(!magazine)
            return;

        StartSnapping(magazine);
    }

    private void StartSnapping(Magazine magazine)
    {
        currentMagazine = magazine;
    }

    private void StopSnapping()
    {
        currentMagazine.model.localPosition = Vector3.zero;
        currentMagazine = null;
    }
}
