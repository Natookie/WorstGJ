using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public enum ForwardAxis { Right, Up }

    [Header("REFERENCES")]
    [SerializeField] private Transform interactFacing;

    [Header("SETTINGS")]
    [SerializeField] private ForwardAxis forwardAxis = ForwardAxis.Right;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float scanInterval = 0.1f;
    [SerializeField] private Vector2 originOffset = Vector2.zero;
    [SerializeField] private LayerMask interactableMask = ~0;
    [SerializeField] private InteractionPromptUI promptUI;

    private readonly RaycastHit2D[] _buffer = new RaycastHit2D[8];
    private ContactFilter2D _filter;
    private IInteractable current;
    private Vector2 _lastFacing;
    private float _timer;

    private Vector2 Facing =>
        forwardAxis == ForwardAxis.Right ? (Vector2)interactFacing.transform.right
        : (Vector2)interactFacing.transform.up;

    void Awake(){
        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = interactableMask,
            useTriggers = true
        };
    }

    void Start(){
        Debug.Assert(interactFacing, "interactFacing is missing");
    }

    void Update(){
        if(IsSuppressed()){
            if(current != null){
                current.OnUnhover(gameObject);
                current = null;
                Refresh();
            }
            return;
        }

        _timer -= Time.deltaTime;

        Vector2 facing = Facing;
        bool turned = (facing - _lastFacing).sqrMagnitude > 0.0001f;

        if(_timer <= 0f || turned){
            _timer = scanInterval;
            _lastFacing = facing;
            UpdateTarget(facing);
        }

        if(current != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            current.OnInteract(gameObject);
    }

    bool IsSuppressed(){
        if(CameraController.Instance != null && CameraController.Instance.IsInScreenMode) return true;
        if(DialogueManager.Instance != null && DialogueManager.Instance.IsVisible()) return true;
        return false;
    }

    void UpdateTarget(Vector2 facing){
        Vector2 origin = (Vector2)interactFacing.transform.position + originOffset;
        int count = Physics2D.Raycast(origin, facing, _filter, _buffer, range);

        IInteractable found = null;

        for(int i = 0; i < count; i++){
            var hit = _buffer[i].collider;
            if(hit.transform.IsChildOf(interactFacing.transform)) continue;
            if(!hit.TryGetComponent(out IInteractable interactable)) continue;
            if(!interactable.CanInteract) continue;

            found = interactable;
            break;
        }

        // Check is found interacable is the same as current
        if (ReferenceEquals(found, current)) return;

        // Change current interacable
        if (!current.IsUnityNull()) current.OnUnhover(gameObject);
        if (!found.IsUnityNull()) found.OnHover(gameObject);

        current = found;

        Refresh();
    }

    public void Refresh() => promptUI?.Show(current?.promptMessage);

    void OnDrawGizmosSelected(){
        if(!interactFacing) return;

        Gizmos.color = Color.yellow;
        Vector2 origin = (Vector2)interactFacing.transform.position + originOffset;
        Gizmos.DrawLine(origin, origin + Facing * range);
    }
}