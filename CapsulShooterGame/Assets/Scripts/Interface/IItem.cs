using UnityEngine;
using Items;

public interface IItem
{
    string Name { get; }
    Sprite Icon { get; }
    string Description { get; }
    int Price { get; }
    ItemType ItemType { get; }
}
