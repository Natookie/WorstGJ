using UnityEngine;

public class Device : BaseDevice
{
    [Header("INTERACT")]
    [SerializeField] private ScreenDefinition screen;
    [SerializeField] private string promptOverride;
    private ScreenRegistry registry;

    [Header("VISUAL")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite deviceSprite;

    private PlayerMovement _activePlayer;
    private bool _inUse;

    void Awake(){
        if(sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if(sr != null && deviceSprite != null) sr.sprite = deviceSprite;
    }

    void Start(){
        registry = ScreenRegistry.Instance;
    }

    public override void OnInteract(GameObject interactor){
        if(_inUse) return;
        if(screen == null || registry == null){
            Debug.LogWarning($"[Device] '{name}' missing screen/registry");
            return;
        }

        Transform node = registry.GetNode(screen);
        if(node == null){
            Debug.LogWarning($"[Device] '{name}' screen node not found for '{screen.Id}'");
            return;
        }

        if(!interactor.TryGetComponent(out PlayerMovement player)) return;
        if(!interactor.TryGetComponent(out PlayerInteractor interactorUi)) return;

        _inUse = true;
        _activePlayer = player;

        player.EnterScreenMode(node);
        interactorUi.Refresh();
    }

    public void CloseDevice(){
        if(!_inUse) return;

        _inUse = false;

        if(_activePlayer != null){
            _activePlayer.ExitScreenMode();
            _activePlayer = null;
        }

        OnDeviceClosed();
    }

    protected virtual void OnDeviceClosed(){ }
    protected override bool GetCanInteract() => !_inUse;
    protected override string GetPromptMessage(){
        if(_inUse) return string.Empty;
        return string.IsNullOrEmpty(promptOverride) ? base.GetPromptMessage() : promptOverride;
    }
}