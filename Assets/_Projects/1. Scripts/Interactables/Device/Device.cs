using UnityEngine;

public class Device : BaseDevice
{
    [Header("INTERACT")]
    [SerializeField] private ScreenDefinition screen;
    [SerializeField] private string promptOverride;
    private ScreenRegistry registry;

    [Header("ITEM")]
    [SerializeField] private bool isItemNeededToOperate = false;
    [SerializeField] private ItemData neededItemData;
    [SerializeField] private string neededItemSuffix = " - Need Item";

    [Header("VISUAL")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite deviceSprite;

    private bool _inUse;
    private bool _isNeededItemFulfilled = false;

    void Awake(){
        if(sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if(sr != null && deviceSprite != null) sr.sprite = deviceSprite;
    }

    void Start() => registry = ScreenRegistry.Instance;

    void OnDisable(){
        if(CameraController.Instance != null) CameraController.Instance.OnScreenModeExit -= HandleScreenModeExit;
    }

    void HandleScreenModeExit(){
        if(!_inUse) return;

        _inUse = false;

        if(CameraController.Instance != null)
            CameraController.Instance.OnScreenModeExit -= HandleScreenModeExit;

        OnDeviceClosed();
    }

    public override void OnInteract(GameObject interactor){
        if(_inUse) return;

        if (isItemNeededToOperate) if (!_isNeededItemFulfilled) return;

        if(screen == null || registry == null){
            Debug.LogWarning($"[Device] '{name}' missing screen/registry");
            return;
        }

        Transform node = registry.GetNode(screen);
        if(node == null){
            Debug.LogWarning($"[Device] '{name}' screen node not found for '{screen.Id}'");
            return;
        }

        if(!interactor.TryGetComponent(out PlayerInteractor interactorUi)) return;

        _inUse = true;

        CameraController.Instance.OnScreenModeExit -= HandleScreenModeExit;
        CameraController.Instance.OnScreenModeExit += HandleScreenModeExit;

        CameraController.Instance.EnterScreenMode(node);
        interactorUi.Refresh();
    }

    public override bool TouchedByItem(string itemName)
    {
        if (!isItemNeededToOperate) return false;
        if (_isNeededItemFulfilled) return false;

        // Check neededItemData
        if (!neededItemData)
        {
            Debug.LogWarning($"neededItemData is empty");
            return false;
        }

        if (itemName == neededItemData.itemName)
        {
            _isNeededItemFulfilled = true;
            return true;
        }
        else return false;
    }

    protected virtual void OnDeviceClosed(){ }
    protected override bool GetCanInteract() => !_inUse;
    protected override string GetPromptMessage()
    {
        if(_inUse) return string.Empty;

        string returnMessage = "";

        if (string.IsNullOrEmpty(promptOverride)) returnMessage += base.GetPromptMessage();
        else returnMessage += promptOverride;

        if (isItemNeededToOperate)
            if (!_isNeededItemFulfilled) returnMessage += neededItemSuffix;

        return returnMessage;
    }
}