using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager singleton;

    public Transform[] targets;

    private void Awake()
    {
        singleton = this;
    }
}
