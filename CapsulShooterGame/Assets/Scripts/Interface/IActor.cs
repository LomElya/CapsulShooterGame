using UnityEngine;
using Items;

public interface IActor
{
    string Name { get; }
    Sprite Icon { get; }
    string Description { get; }
}
