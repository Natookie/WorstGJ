using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerMovement player;

    private bool inUse;
    private bool canInteract = true;

    public bool CanInteract => canInteract && !inUse;
    public Transform Transform => transform;
    public string promptMessage => inUse ? string.Empty : "Use Computer";

    public void OnHover(GameObject interactor){}
    public void OnUnhover(GameObject interactor){}

    public void OnInteract(GameObject interactor){
        if(inUse) return;

        if(!interactor.TryGetComponent(out PlayerMovement p)) return;
        if(!interactor.TryGetComponent(out PlayerInteractor pi)) return;

        inUse = true;
        canInteract = false;

        p.EnterComputerMode();
        pi.Refresh();
    }

    public void CloseComputer(){
        if(!inUse) return;

        inUse = false;
        canInteract = true;
        if(player != null) player.ExitComputerMode();
        if(TextFieldManager.IsAnyFocused){
            TextFieldManager.ClearAll("loginemployeehub");
            TextFieldManager.DefocusAll();
        }
    }
}