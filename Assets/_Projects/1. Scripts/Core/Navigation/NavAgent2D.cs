using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Path-following state for a 2D agent on a <see cref="TilemapNav2D"/>.
/// Computes and stores the A* path and hands out waypoints, but does NOT move anything.
/// Pair it with <see cref="NavAgentMover2D"/> (or your own movement script) to walk the path.
/// </summary>
public class NavAgent2D : MonoBehaviour
{
    public TilemapNav2D tilemapNav;

    [Header("Repathing")]
    [Tooltip("Recompute the path if the next waypoint becomes blocked (e.g. a dynamic obstacle appeared).")]
    [SerializeField] private bool repathWhenBlocked = true;

    [Header("Debug")]
    [SerializeField] private bool drawPath = true;

    public event Action<Vector3> DestinationReached;
    public event Action PathFailed;

    public bool HasPath => pathIndex < path.Count;
    public Vector3 Destination { get; private set; }

    private readonly List<Vector3> path = new List<Vector3>();
    private int pathIndex;

    private void Awake()
    {
        Destination = transform.position;

        if (tilemapNav == null)
            tilemapNav = FindFirstObjectByType<TilemapNav2D>();
    }

    /// <summary>Requests a path to the world position. Returns false if none exists.</summary>
    public bool SetDestination(Vector3 worldTarget)
    {
        if (tilemapNav == null)
        {
            Debug.LogWarning("NavAgent2D: no TilemapNav2D assigned or found in scene.", this);
            return false;
        }

        if (!tilemapNav.FindPath(transform.position, worldTarget, path))
        {
            path.Clear();
            pathIndex = 0;
            PathFailed?.Invoke();
            return false;
        }

        pathIndex = 0;
        Destination = path[path.Count - 1];
        return true;
    }

    /// <summary>Clears the current path. The agent will stop receiving waypoints.</summary>
    public void Stop()
    {
        path.Clear();
        pathIndex = 0;
    }

    /// <summary>
    /// Gets the waypoint the agent should currently head towards.
    /// Repaths first if that waypoint has become blocked. Returns false if there is no path.
    /// </summary>
    public bool TryGetCurrentWaypoint(out Vector3 waypoint)
    {
        waypoint = default;
        if (!HasPath) return false;

        if (repathWhenBlocked && !tilemapNav.IsWalkable(tilemapNav.WorldToCell(path[pathIndex])))
        {
            if (!SetDestination(Destination) || !HasPath)
                return false;
        }

        waypoint = path[pathIndex];
        return true;
    }

    /// <summary>Call when the current waypoint has been reached. Fires DestinationReached after the last one.</summary>
    public void AdvanceWaypoint()
    {
        if (!HasPath) return;

        pathIndex++;

        if (pathIndex >= path.Count)
        {
            path.Clear();
            pathIndex = 0;
            DestinationReached?.Invoke(Destination);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawPath || !HasPath) return;

        Gizmos.color = Color.cyan;
        Vector3 prev = transform.position;
        for (int i = pathIndex; i < path.Count; i++)
        {
            Gizmos.DrawLine(prev, path[i]);
            Gizmos.DrawSphere(path[i], 0.06f);
            prev = path[i];
        }
    }
}