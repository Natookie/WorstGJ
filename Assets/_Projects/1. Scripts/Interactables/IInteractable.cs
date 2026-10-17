using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    string promptMessage { get;}
    
    void OnHover(GameObject interactor);
    void OnUnhover(GameObject interactor);
    void OnInteract(GameObject interactor);
}