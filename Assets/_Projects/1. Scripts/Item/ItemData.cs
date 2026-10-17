using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
}
