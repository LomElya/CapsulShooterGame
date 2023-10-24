using UnityEngine;

[CreateAssetMenu(menuName = "Actor")]
public class AssetActor : ScriptableObject, IActor
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;
    [SerializeField] private Perosnage _personagePrefab;
    [SerializeField] private Death _deathPrefab;

    public string Name => _name;
    public Sprite Icon => _icon;
    public string Description => _description;
    public Perosnage PersonagePrefab => _personagePrefab;
    public Death DeathPrefab => _deathPrefab;
}
