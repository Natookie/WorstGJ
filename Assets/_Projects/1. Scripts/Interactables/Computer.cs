using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerMovement player;

    private bool inUse;

    public bool CanInteract => true;
    public Transform Transform => transform;
    public string promptMessage => inUse ? "Exit" : "Use Computer";

    public void OnInteract(GameObject interactor){
        if(!interactor.TryGetComponent(out PlayerMovement p)) return;
        if(!interactor.TryGetComponent(out PlayerInteractor pi)) return;

        inUse = !inUse;

        if(inUse) p.EnterComputerMode();
        else      p.ExitComputerMode();

        pi.Refresh();
    }
}