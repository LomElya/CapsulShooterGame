using UnityEngine;
using Items;

[CreateAssetMenu(menuName = "Item")]
public class AssetItem : ScriptableObject, IItem
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;
    [SerializeField] private int _price;
    [SerializeField] private ItemType _itemType;

    public string Name => _name;
    public Sprite Icon => _icon;
    public string Description => _description;
    public int Price => _price;
    public ItemType ItemType => _itemType;

}
