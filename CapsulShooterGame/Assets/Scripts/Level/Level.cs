using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform _initialPosition;
    [SerializeField] private HardLevel _hardLevelPrefab;
    public Vector3 InitialPosition => _initialPosition.position;
    public HardLevel HardLevelPrefab => _hardLevelPrefab;
}
