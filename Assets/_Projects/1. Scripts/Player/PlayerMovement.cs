using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxConfinement2D confinement;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private RectTransform frameRect;

    [Header("MOVEMENT")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float linearDrag = 0.8f;
    [SerializeField] private float turnSpeed = 720f;

    [Header("KNOCKBACK")]
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float minKnockbackImpact = 1f;

    public bool isEnabled = true;

    static readonly int HashX     = Animator.StringToHash("X");
    static readonly int HashY     = Animator.StringToHash("Y");
    static readonly int HashSpeed = Animator.StringToHash("Speed");

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

    void Update(){
        if(IsBusy){
            _forward = false;
            UpdateAnimator();
            return;
        }
        if (!isEnabled) return;

        _forward = Keyboard.current.wKey.isPressed;

        if(!_forward){
            Vector2 mouseFacing = GetMouseDirection();
            if(mouseFacing.sqrMagnitude > 0.0001f) _facing = mouseFacing;
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0f, 0f, Mathf.Atan2(_facing.y, _facing.x) * Mathf.Rad2Deg - 90f),
            turnSpeed * Time.deltaTime);

        UpdateAnimator();
    }

    void FixedUpdate(){
        if(IsBusy){
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (!isEnabled) return;

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
        if(worldCamera == null || frameRect == null || Mouse.current == null) return Vector2.zero;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 offset = screenPos - screenCenter;

        Vector2 viewport = new Vector2(
            offset.x / frameRect.rect.width + 0.5f,
            offset.y / frameRect.rect.height + 0.5f);

        Ray ray = worldCamera.ViewportPointToRay(viewport);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        if(!plane.Raycast(ray, out float dist)) return Vector2.zero;

        Vector3 world = ray.GetPoint(dist);
        Vector2 dir = (Vector2)(world - transform.position);
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.zero;
    }

    void UpdateAnimator(){
        if(animator == null) return;

        animator.SetFloat(HashX, _facing.x);
        animator.SetFloat(HashY, _facing.y);
        animator.SetFloat(HashSpeed, rb.linearVelocity.sqrMagnitude);
    }

    public Vector2 Velocity => rb.linearVelocity;
    public Vector2 Facing   => _facing;

    public void EnterComputerMode()
    {
        isEnabled = false;
        cameraController.EnterComputerMode();
    }
    public void ExitComputerMode()
    {
        isEnabled = true;
        cameraController.ExitComputerMode();
    }
}