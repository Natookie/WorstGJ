using UnityEngine;

public class PickupableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    public bool CanInteract => true;
    public string promptMessage => "Grab";
    
    private void Start()
    {
        Debug.Assert(itemData, "itemData is empty");
    }

    public void OnInteract(GameObject interactor){
        if(!interactor.TryGetComponent(out PlayerInteractor playerInteractor)) return;
        if(!interactor.TryGetComponent(out PlayerInventory playerInventory)) return;

        if (!playerInventory.HasItem)
        {
            playerInventory.GrabItem(itemData);
            playerInteractor.Refresh();
            Destroy(gameObject);
        }
    }
}
