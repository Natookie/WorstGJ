using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Moves a GameObject towards the waypoints provided by a <see cref="NavAgent2D"/>.
/// Uses a Rigidbody2D if present, otherwise moves the Transform.
/// Swap this out for your own mover (animation-driven, physics forces, etc.) without touching pathfinding.
/// </summary>
[RequireComponent(typeof(NavAgent2D))]
public class NPCMovementComponent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [Tooltip("Distance at which a waypoint counts as reached.")]
    [SerializeField] private float waypointTolerance = 0.05f;

    [Header("Rotation")]
    [Tooltip("Rotate the sprite to face the movement direction.")]
    [SerializeField] private bool faceMoveDirection = false;
    [SerializeField] private float rotationOffset = -90f;

    [Header("Events")]
    [Tooltip("Fired when the NPC arrives at the move point (final destination) it was sent to. " +
             "Not fired when movement is interrupted by StopMove(), or when the path fails.")]
    [SerializeField] private UnityEvent<Vector3> onMovePointReached = new UnityEvent<Vector3>();

    public UnityEvent<Vector3> MovePointReached => onMovePointReached;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public Vector2 Velocity { get; private set; }
    public bool IsMoving => agent != null && agent.HasPath;
    public TilemapNav2D TilemapNav => agent.tilemapNav;

    private NavAgent2D agent;
    private Rigidbody2D body;

    private void Awake()
    {
        agent = GetComponent<NavAgent2D>();
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        agent.DestinationReached += HandleDestinationReached;
    }

    private void OnDisable()
    {
        agent.DestinationReached -= HandleDestinationReached;
    }

    /// <summary>Sends the NPC to a world position. Returns false if no path exists.</summary>
    public bool MoveTo(Vector3 worldTarget)
    {
        return agent.SetDestination(worldTarget);
    }

    /// <summary>Stops moving immediately and discards the current path.</summary>
    public void StopMove()
    {
        agent.Stop();
        HaltBody();
    }

    private void HandleDestinationReached(Vector3 point)
    {
        onMovePointReached.Invoke(point);
    }

    private void FixedUpdate()
    {
        if (!agent.TryGetCurrentWaypoint(out Vector3 waypoint))
        {
            HaltBody();
            return;
        }

        Vector2 position = body != null ? body.position : (Vector2)transform.position;
        Vector2 toTarget = (Vector2)waypoint - position;
        float step = moveSpeed * Time.fixedDeltaTime;

        // Close enough: snap to the waypoint and move on to the next one.
        if (toTarget.magnitude <= Mathf.Max(step, waypointTolerance))
        {
            ApplyPosition(waypoint);
            Velocity = Vector2.zero;
            agent.AdvanceWaypoint(); // fires DestinationReached (-> OnMovePointReached) after the last waypoint
            return;
        }

        Vector2 dir = toTarget.normalized;
        Velocity = dir * moveSpeed;
        ApplyPosition(position + dir * step);

        if (faceMoveDirection)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void HaltBody()
    {
        Velocity = Vector2.zero;
        if (body != null && body.bodyType == RigidbodyType2D.Dynamic)
            body.linearVelocity = Vector2.zero;
    }

    private void ApplyPosition(Vector2 pos)
    {
        if (body != null) body.MovePosition(pos);
        else transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}