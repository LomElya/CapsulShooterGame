using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] private List<Enemy> _enemyList = new List<Enemy>();
    private List<IMoveble> _moverList = new List<IMoveble>();
    public List<Transform> _pathNodes = new List<Transform>();

    private void Start()
    {
        foreach (Enemy enemy in _enemyList)
        {
            if (enemy.TryGetComponent(out IMoveble moveble))
            {
                _moverList.Add(moveble);
            }
        }

        foreach (IMoveble move in _moverList)
        {
            move.PatrolPath = this;
        }
    }
    
    private float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
    {
        if (destinationNodeIndex < 0 || destinationNodeIndex >= _pathNodes.Count ||
            _pathNodes[destinationNodeIndex] == null)
        {
            return -1f;
        }

        return (_pathNodes[destinationNodeIndex].position - origin).magnitude;
    }

    public Vector3 GetPositionOfPathNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex >= _pathNodes.Count || _pathNodes[nodeIndex] == null)
        {
            return Vector3.zero;
        }

        return _pathNodes[nodeIndex].position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < _pathNodes.Count; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex >= _pathNodes.Count)
            {
                nextIndex -= _pathNodes.Count;
            }

            Gizmos.DrawLine(_pathNodes[i].position, _pathNodes[nextIndex].position);
            Gizmos.DrawSphere(_pathNodes[i].position, 0.1f);
        }
    }
}
