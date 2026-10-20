using UnityEngine;
using UnityEngine.InputSystem;
using Nova;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private BoxConfinement2D confinement;
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

    public bool isEnabled = true;

    private Vector2 _facing = Vector2.up;
    private bool _forward;

    public bool IsBusy => (cameraController != null && cameraController.IsTransitioning) || DialogueManager.Instance.IsVisible();

    void Awake(){
        rb.gravityScale = 0f;
        rb.linearDamping = linearDrag;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Start()
    {
        Debug.Assert(rb, "rb is missing");
        Debug.Assert(playerVisual, "playerVisual is missing");
        Debug.Assert(confinement, "confinement is missing");
        Debug.Assert(interactFacing, "interactFacing is missing");
    }

    void Update(){
        if(IsBusy){
            _forward = false;
            playerVisual.UpdateAnimation(_facing, rb.linearVelocity.sqrMagnitude);
            return;
        }
        if(!isEnabled) return;

        _forward = Keyboard.current.wKey.isPressed;

        if(!_forward){
            Vector2 mouseFacing = GetMouseDirection();
            if(mouseFacing.sqrMagnitude > 0.0001f) _facing = mouseFacing;
        }

        // transform.rotation = Quaternion.RotateTowards(
        //     transform.rotation,
        //     Quaternion.Euler(0f, 0f, Mathf.Atan2(_facing.y, _facing.x) * Mathf.Rad2Deg - 90f),
        //     turnSpeed * Time.deltaTime);
        
        // Update interactFacing direction
        interactFacing.rotation = Quaternion.RotateTowards(
            interactFacing.rotation,
            Quaternion.Euler(0f, 0f, Mathf.Atan2(_facing.y, _facing.x) * Mathf.Rad2Deg - 90f),
            turnSpeed * Time.deltaTime);

        playerVisual.UpdateAnimation(_facing, rb.linearVelocity.sqrMagnitude);
    }

    void FixedUpdate(){
        if(IsBusy){
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if(!isEnabled) return;

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

        var hits = new System.Collections.Generic.List<UIBlockHit>();
        Interaction.RaycastAll(uiRay, hits);

        UIBlockHit hit = default;
        bool found = false;
        foreach(var h in hits){
            if(h.UIBlock == rtDisplay){ hit = h; found = true; break; }
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

    public Vector2 Velocity => rb.linearVelocity;
    public Vector2 Facing   => _facing;
    
    public void EnterScreenMode(string deviceName){
        isEnabled = false;
        cameraController.EnterScreenMode(deviceName);
    }
    public void ExitScreenMode(){
        isEnabled = true;
        cameraController.ExitScreenMode();
    }
}