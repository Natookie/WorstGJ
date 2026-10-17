using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PickupableItem pickupableItemPrefab;
    [SerializeField] private string dropCommand = "drop";

    public bool HasItem => holdingItemData != null;

    private ItemData holdingItemData;

    private void Start()
    {
        Debug.Assert(pickupableItemPrefab, "pickupableItemPrefab is empty");

        EventBus.TypeCommandEmitted.AddListener(OnTypeCommandEmitted);
    }

    public void GrabItem(ItemData newItemData)
    {
        holdingItemData = newItemData;

        // TODO: Erase if unused
        Debug.Log($"Grab {holdingItemData.itemName}");
    }

    public void DropItem()
    {
        if (!HasItem) return;

        // TODO: Erase if unused
        Debug.Log($"Drop {holdingItemData.itemName}");

        PickupableItem pickupableItemInstance = Instantiate(pickupableItemPrefab);
        pickupableItemInstance.transform.position = transform.position;

        // TODO: Set pickupableItemInstance parent if needed

        holdingItemData = null;
    }

    private void OnTypeCommandEmitted(string command)
    {
        if (dropCommand != command) return;

        DropItem();
    }
}
