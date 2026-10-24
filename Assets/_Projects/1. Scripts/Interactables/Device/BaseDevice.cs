using UnityEngine;

public class BaseDevice : MonoBehaviour, IInteractable
{
    public string deviceName;

    public bool CanInteract => GetCanInteract();
    public Transform Transform => transform;
    public string promptMessage => GetPromptMessage();

    virtual public void OnHover(GameObject interactor){}
    virtual public void OnUnhover(GameObject interactor){}
    virtual public void OnInteract(GameObject interactor) {Debug.Log("Interacted");}

    /// <summary>
    /// Call function when PickupableItem collided with device.
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns>Bool for touched item gets destroyed or not</returns>
    public virtual bool TouchedByItem(string itemName) {return false;}

    protected virtual bool GetCanInteract() => true;
    protected virtual string GetPromptMessage() => "Device";
}
