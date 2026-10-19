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

    protected virtual bool GetCanInteract() => true;
    protected virtual string GetPromptMessage() => "Device";
}
