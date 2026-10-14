using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    void OnInteract(GameObject interactor);
    string promptMessage { get;}
}