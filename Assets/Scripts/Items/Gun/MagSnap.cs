using UnityEngine;

public class MagSnap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Gun gun = default;
    [SerializeField] private Transform destination = default;

    private Magazine currentMagazine;
    private bool isAttached;
    private float distToDestination;

    private void Awake()
    {
        distToDestination = Vector3.Distance(transform.position, destination.position);
    }

    private void LateUpdate()
    {
        if(!currentMagazine)
            return;

        if(!isAttached)
        {
            Vector3 relativeMagPos = destination.InverseTransformPoint(currentMagazine.transform.position);
            float interp = relativeMagPos.z * -distToDestination;
            if(interp > 1f + 0.5f) // 0.5f buffer
            {
                StopSnapping();
                return;
            }
            if(!currentMagazine.grabbable.isGrabbed)
            {
                if(interp > 0.1f)
                {
                    StopSnapping();
                    return;
                }
                // Load magazine
                currentMagazine.transform.SetParent(destination);
                //currentMagazine.coll.isTrigger = true;
                currentMagazine.rb.isKinematic = true;
                currentMagazine.rb.velocity = Vector3.zero;
                currentMagazine.rb.angularVelocity = Vector3.zero;
                isAttached = true;
                gun.magazine = currentMagazine;
                currentMagazine.model.position = destination.position;
                currentMagazine.model.eulerAngles = transform.eulerAngles;
                return;
            }
            currentMagazine.model.position = Vector3.Lerp(destination.position, transform.position, interp);
            currentMagazine.model.eulerAngles = transform.eulerAngles;
        }
        else
        {
            if(currentMagazine.grabbable.isGrabbed)
            {
                //currentMagazine.coll.isTrigger = false;
                //currentMagazine.rb.isKinematic = false;
                //currentMagazine.rb.useGravity = false;
                isAttached = false;
                gun.magazine = null;
            }
        }
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
        magazine.rb.useGravity = false;
    }

    private void StopSnapping()
    {
        currentMagazine.rb.useGravity = true;
        currentMagazine.model.localPosition = Vector3.zero;
        currentMagazine = null;
    }
}
