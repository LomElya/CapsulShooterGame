using UnityEngine;

public interface IMoveble
{
    public float MaxSpeed { get; }
    public float PathReachingRadius { get; }
    public int PathDestinationNodeIndex { get; set; }
    public PatrolPath PatrolPath { get; set; }

    public bool IsPathValid();

    public Vector3 GetDestinationOnPath();

    public void UpdatePathDestination(bool inverseOrder = false);
    public void SetNavDestination(Vector3 destination);
}
