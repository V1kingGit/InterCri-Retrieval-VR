using UnityEngine;

public class Npc : MonoBehaviour
{
    public static readonly Vector3 invalidVector = Vector3.one * -99999f;

    public NpcMovement movement;
    public NpcCombat combat;
}
