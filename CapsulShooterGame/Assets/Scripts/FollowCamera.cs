using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform _target;


    public void Follow(Transform playerTransform) => _target = playerTransform; 
    private void LateUpdate()
    {
        if (_target == null)
            return;
        
        transform.position = _target.position;
    }
}
