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

        float interp = Vector3.Distance(currentMagazine.transform.position, destination.position) / distToDestination;
        if(interp > 1.1f)
        {
            currentMagazine = null;
            currentMagazine.model.localPosition = Vector3.zero;
            return;
        }
        currentMagazine.model.position = Vector3.Lerp(destination.position, transform.position, interp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentMagazine)
            return;

        Magazine magazine = other.GetComponent<Magazine>();
        if(!magazine)
            return;

        currentMagazine = magazine;
    }
}
