using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PickupableItem pickupableItemPrefab;

    public bool HasItem => holdingItemData != null;

    private ItemData holdingItemData;

    public void GrabItem(ItemData newItemData)
    {
        holdingItemData = newItemData;
        Debug.Log($"Grab {holdingItemData.itemName}");
    }

    public void DropItem()
    {
        
    }
}
