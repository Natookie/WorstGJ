using UnityEngine;

public class Computer : BaseDevice
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private string promptOveride;

    private bool inUse;
    private bool canInteract = true;

    public override void OnInteract(GameObject interactor){
        if(inUse) return;

        if(!interactor.TryGetComponent(out PlayerMovement p)) return;
        if(!interactor.TryGetComponent(out PlayerInteractor pi)) return;

        inUse = true;
        canInteract = false;

        p.EnterScreenMode(deviceName);
        pi.Refresh();
    }

    public void CloseComputer(){
        if(!inUse) return;

        inUse = false;
        canInteract = true;
        if(player != null) player.ExitScreenMode();
        if(TextFieldManager.IsAnyFocused){
            TextFieldManager.ClearAll("loginemployeehub");
            TextFieldManager.DefocusAll();
        }
    }

    protected override bool GetCanInteract() => canInteract && !inUse;
    protected override string GetPromptMessage() => inUse ? string.Empty : promptOveride;
}