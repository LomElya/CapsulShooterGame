using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class TurretLaser : Enemy, IMoveble
{
    [SerializeField] private float _pathReachingRadius = 2f;
    [SerializeField] private float _maxSpeed;

    public float PathReachingRadius => _pathReachingRadius;
    public float MaxSpeed => _maxSpeed;

    public int PathDestinationNodeIndex { get; set; }
    public PatrolPath PatrolPath { get; set; }

    protected override void Update()
    {
        if (isGameStop)
            return;

        _currentWeapon.HandleUseInputs(true, false, false);
    }
    public bool IsPathValid()
    {
        return PatrolPath && PatrolPath._pathNodes.Count > 0;
    }

    public Vector3 GetDestinationOnPath()
    {
        if (IsPathValid())
        {
            return PatrolPath.GetPositionOfPathNode(PathDestinationNodeIndex);
        }
        else
        {
            return transform.position;
        }
    }

    public void UpdatePathDestination(bool inverseOrder = false)
    {
        if (IsPathValid())
        {
            /// Если дошел до конца
            if ((transform.position - GetDestinationOnPath()).magnitude <= _pathReachingRadius)
            {
                /// Изменить индекс маршрута
                PathDestinationNodeIndex =
                    inverseOrder ? (PathDestinationNodeIndex - 1) : (PathDestinationNodeIndex + 1);
                if (PathDestinationNodeIndex < 0)
                {
                    PathDestinationNodeIndex += PatrolPath._pathNodes.Count;
                }

                if (PathDestinationNodeIndex >= PatrolPath._pathNodes.Count)
                {
                    PathDestinationNodeIndex -= PatrolPath._pathNodes.Count;
                }
            }
        }
    }

    public void SetNavDestination(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, _maxSpeed * Time.deltaTime);
    }
}