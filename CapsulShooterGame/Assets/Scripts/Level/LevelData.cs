using UnityEngine;

[System.Serializable]
public class LevelData
{
    [SerializeField] private int id;
    [SerializeField] private Level _levelPrefab;
    public int ID => id;
    public Level LevelPrefab => _levelPrefab;
    public HardLevel HardLevelPrefab => _levelPrefab.HardLevelPrefab;
    public Vector3 InitialPosition => _levelPrefab.InitialPosition;
}
