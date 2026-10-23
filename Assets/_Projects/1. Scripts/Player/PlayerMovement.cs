using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Nova;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Transform interactFacing;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private UIBlock2D rtDisplay;

    [Header("MOVEMENT")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float linearDrag = 0.8f;
    [SerializeField] private float turnSpeed = 720f;

    [Header("KNOCKBACK")]
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float minKnockbackImpact = 1f;

    private readonly List<UIBlockHit> _hitsBuffer = new();
    private BoxConfinement2D confinement;

    private Vector2 _facing = Vector2.up;
    private bool _forward;
    private bool _externallyDisabled;

    public bool IsEnabled => !_externallyDisabled;

    public bool IsBusy {
        get {
            if(cameraController != null && cameraController.IsTransitioning) return true;
            if(cameraController != null && cameraController.IsInScreenMode) return true;
            if(DialogueManager.Instance != null && DialogueManager.Instance.IsVisible()) return true;
            return false;
        }
    }

    public Vector2 Velocity => rb.linearVelocity;
    public Vector2 Facing => _facing;

    public void SetEnabled(bool value) => _externallyDisabled = !value;

    void Awake(){
        rb.gravityScale = 0f;
        rb.linearDamping = linearDrag;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        Debug.Assert(rb, "rb is missing");
        Debug.Assert(playerVisual, "playerVisual is missing");
        Debug.Assert(interactFacing, "interactFacing is missing");
    }

    void Start(){
        confinement = BoxConfinement2D.Instance;
    }

    void Update(){
        bool busy = IsBusy;

        if(!busy && IsEnabled){
            _forward = Keyboard.current != null && Keyboard.current.wKey.isPressed;

            if(!_forward){
                Vector2 mouseFacing = GetMouseDirection();
                if(mouseFacing.sqrMagnitude > 0.0001f) _facing = mouseFacing;
            }
        }else{
            _forward = false;
        }

        if(!busy){
            interactFacing.rotation = Quaternion.RotateTowards(
                interactFacing.rotation,
                Quaternion.Euler(0f, 0f, Mathf.Atan2(_facing.y, _facing.x) * Mathf.Rad2Deg - 90f),
                turnSpeed * Time.deltaTime);
        }

        float speedSq = rb.linearVelocity.sqrMagnitude;
        playerVisual.UpdateAnimation(_facing, busy ? 0f : speedSq);
    }

    void FixedUpdate(){
        if(IsBusy || !IsEnabled){
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if(_forward) rb.AddForce(_facing * acceleration, ForceMode2D.Force);

        if(confinement != null){
            Vector2 pos = rb.position;
            Vector2 clamped = confinement.ClampBox(pos);
            Vector2 delta = clamped - pos;

            if(delta.sqrMagnitude > 0.000001f){
                Vector2 normal = delta.normalized;
                float into = -Vector2.Dot(rb.linearVelocity, normal);

                if(into > 0f) rb.linearVelocity += normal * into;
                if(into > minKnockbackImpact) rb.linearVelocity += normal * knockbackForce;

                rb.position = clamped;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.relativeVelocity.sqrMagnitude < minKnockbackImpact * minKnockbackImpact) return;

        Vector2 normal = Vector2.zero;
        for(int i = 0; i < collision.contactCount; i++)
            normal += collision.GetContact(i).normal;

        if(normal.sqrMagnitude < 0.0001f) return;
        normal.Normalize();

        rb.linearVelocity += normal * knockbackForce;
    }

    Vector2 GetMouseDirection(){
        if(worldCamera == null || uiCamera == null || rtDisplay == null || Mouse.current == null)
            return Vector2.zero;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray uiRay = uiCamera.ScreenPointToRay(screenPos);

        _hitsBuffer.Clear();
        Interaction.RaycastAll(uiRay, _hitsBuffer);

        bool found = false;
        UIBlockHit hit = default;
        for(int i = 0; i < _hitsBuffer.Count; i++){
            if(_hitsBuffer[i].UIBlock == rtDisplay){
                hit = _hitsBuffer[i];
                found = true;
                break;
            }
        }

        if(!found) return Vector2.zero;

        Vector3 local = rtDisplay.transform.InverseTransformPoint(hit.Position);
        Vector2 size = rtDisplay.CalculatedSize.XY.Value;

        if(size.x <= 0f || size.y <= 0f) return Vector2.zero;

        Vector2 uv = new Vector2(local.x / size.x + 0.5f, local.y / size.y + 0.5f);
        Ray worldRay = worldCamera.ViewportPointToRay(uv);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        if(!plane.Raycast(worldRay, out float dist)) return Vector2.zero;

        Vector3 world = worldRay.GetPoint(dist);
        Vector2 dir = (Vector2)(world - transform.position);
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.zero;
    }
}