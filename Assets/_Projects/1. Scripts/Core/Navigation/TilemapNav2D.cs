using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 2D navigation grid driven by a single Tilemap.
/// Any cell that contains a tile in the navigation tilemap is walkable.
/// Cells without a tile are blocked. Optionally, physics colliders on
/// the obstacle mask can also block otherwise walkable cells.
/// Pathfinding is A*.
/// </summary>
public class TilemapNav2D : MonoBehaviour
{
    [Header("Navigation Tilemap")]
    [Tooltip("Painted cells = walkable. Empty cells = blocked.")]
    [SerializeField] private Tilemap navTilemap;

    [Header("Pathfinding")]
    [SerializeField] private bool allowDiagonal = true;
    [Tooltip("Disallow diagonal moves that squeeze between two blocked cells.")]
    [SerializeField] private bool preventCornerCutting = true;
    [Tooltip("If the start/goal cell isn't walkable, snap to the nearest walkable cell within this many cells.")]
    [SerializeField] private int snapSearchRadius = 5;

    [Header("Optional Obstacles")]
    [Tooltip("Colliders on these layers block a cell even if it has a tile. Leave as Nothing to disable.")]
    [SerializeField] private LayerMask obstacleMask;
    [Range(0.1f, 1f)]
    [SerializeField] private float obstacleCheckScale = 0.8f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    public Tilemap NavTilemap => navTilemap;

    private const int StraightCost = 10;
    private const int DiagonalCost = 14;

    private static readonly Vector3Int[] CardinalDirs =
    {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
    };

    private static readonly Vector3Int[] DiagonalDirs =
    {
        new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    private void Reset()
    {
        navTilemap = GetComponentInChildren<Tilemap>();
    }

    // ---------------------------------------------------------------- queries

    public Vector3Int WorldToCell(Vector3 world) => navTilemap.WorldToCell(world);

    public Vector3 CellToWorldCenter(Vector3Int cell) => navTilemap.GetCellCenterWorld(cell);

    public bool IsWalkable(Vector3Int cell)
    {
        if (navTilemap == null || !navTilemap.HasTile(cell))
            return false;

        if (obstacleMask.value != 0)
        {
            Vector2 center = navTilemap.GetCellCenterWorld(cell);
            Vector2 size = (Vector2)navTilemap.cellSize * obstacleCheckScale;
            if (Physics2D.OverlapBox(center, size, 0f, obstacleMask) != null)
                return false;
        }

        return true;
    }

    public bool IsWalkable(Vector3 world) => IsWalkable(WorldToCell(world));

    /// <summary>Finds the closest walkable cell to <paramref name="origin"/> (ring search).</summary>
    public bool TryGetNearestWalkable(Vector3Int origin, out Vector3Int result)
    {
        if (IsWalkable(origin))
        {
            result = origin;
            return true;
        }

        for (int r = 1; r <= snapSearchRadius; r++)
        {
            bool found = false;
            float best = float.MaxValue;
            result = origin;

            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) != r) continue;
                    var c = new Vector3Int(origin.x + x, origin.y + y, origin.z);
                    if (!IsWalkable(c)) continue;
                    float d = x * x + y * y;
                    if (d < best)
                    {
                        best = d;
                        result = c;
                        found = true;
                    }
                }
            }

            if (found) return true;
        }

        result = origin;
        return false;
    }

    // ----------------------------------------------------------- random points

    /// <summary>
    /// Picks a uniformly random walkable cell anywhere on the navigation tilemap
    /// and returns its center in world space.
    /// </summary>
    public bool TryGetRandomWalkablePosition(out Vector3 position)
    {
        position = default;
        if (navTilemap == null) return false;

        BoundsInt bounds = navTilemap.cellBounds;
        if (bounds.size.x <= 0 || bounds.size.y <= 0) return false;

        // Fast path: rejection sampling works well when most of the bounds is walkable.
        for (int i = 0; i < 30; i++)
        {
            var cell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                bounds.zMin);

            if (IsWalkable(cell))
            {
                position = CellToWorldCenter(cell);
                return true;
            }
        }

        // Fallback for sparse maps: scan every cell and pick uniformly (reservoir sampling).
        int count = 0;
        Vector3Int picked = default;
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!IsWalkable(cell)) continue;
            count++;
            if (Random.Range(0, count) == 0) picked = cell;
        }

        if (count == 0) return false;

        position = CellToWorldCenter(picked);
        return true;
    }

    /// <summary>
    /// Picks a uniformly random walkable cell within <paramref name="radius"/> world units of
    /// <paramref name="center"/>. Handy for wandering NPCs.
    /// </summary>
    public bool TryGetRandomWalkablePosition(Vector3 center, float radius, out Vector3 position)
    {
        position = default;
        if (navTilemap == null) return false;

        Vector3Int centerCell = WorldToCell(center);
        Vector3 cellSize = navTilemap.cellSize;
        int rangeX = Mathf.CeilToInt(radius / Mathf.Abs(cellSize.x));
        int rangeY = Mathf.CeilToInt(radius / Mathf.Abs(cellSize.y));
        float sqrRadius = radius * radius;

        int count = 0;
        Vector3Int picked = default;

        for (int x = -rangeX; x <= rangeX; x++)
        {
            for (int y = -rangeY; y <= rangeY; y++)
            {
                var cell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);
                if (!IsWalkable(cell)) continue;

                Vector3 cellCenter = CellToWorldCenter(cell);
                if (((Vector2)(cellCenter - center)).sqrMagnitude > sqrRadius) continue;

                count++;
                if (Random.Range(0, count) == 0) picked = cell;
            }
        }

        if (count == 0) return false;

        position = CellToWorldCenter(picked);
        return true;
    }

    // ------------------------------------------------------------- pathfinding

    /// <summary>
    /// Finds a path between two world positions. The result is a list of cell-center
    /// world positions (the start cell is omitted, the goal cell is last).
    /// </summary>
    public bool FindPath(Vector3 startWorld, Vector3 goalWorld, List<Vector3> path)
    {
        path.Clear();
        if (navTilemap == null) return false;

        if (!TryGetNearestWalkable(WorldToCell(startWorld), out Vector3Int start)) return false;
        if (!TryGetNearestWalkable(WorldToCell(goalWorld), out Vector3Int goal)) return false;

        if (start == goal)
        {
            path.Add(CellToWorldCenter(goal));
            return true;
        }

        var gScore = new Dictionary<Vector3Int, int> { [start] = 0 };
        var parent = new Dictionary<Vector3Int, Vector3Int>();
        var closed = new HashSet<Vector3Int>();
        var open = new MinHeap();
        open.Push(start, Heuristic(start, goal));

        while (open.Count > 0)
        {
            Vector3Int current = open.Pop();
            if (!closed.Add(current)) continue; // stale heap entry

            if (current == goal)
            {
                BuildPath(parent, start, goal, path);
                return true;
            }

            int currentG = gScore[current];

            for (int i = 0; i < CardinalDirs.Length; i++)
                Relax(current, CardinalDirs[i], StraightCost, currentG, goal, gScore, parent, closed, open);

            if (allowDiagonal)
            {
                for (int i = 0; i < DiagonalDirs.Length; i++)
                {
                    Vector3Int d = DiagonalDirs[i];
                    if (preventCornerCutting &&
                        (!IsWalkable(current + new Vector3Int(d.x, 0, 0)) ||
                         !IsWalkable(current + new Vector3Int(0, d.y, 0))))
                        continue;

                    Relax(current, d, DiagonalCost, currentG, goal, gScore, parent, closed, open);
                }
            }
        }

        return false; // no path
    }

    private void Relax(Vector3Int current, Vector3Int dir, int stepCost, int currentG, Vector3Int goal,
        Dictionary<Vector3Int, int> gScore, Dictionary<Vector3Int, Vector3Int> parent,
        HashSet<Vector3Int> closed, MinHeap open)
    {
        Vector3Int next = current + dir;
        if (closed.Contains(next) || !IsWalkable(next)) return;

        int tentative = currentG + stepCost;
        if (gScore.TryGetValue(next, out int existing) && tentative >= existing) return;

        gScore[next] = tentative;
        parent[next] = current;
        open.Push(next, tentative + Heuristic(next, goal));
    }

    private static int Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        // Octile distance
        return StraightCost * (dx + dy) + (DiagonalCost - 2 * StraightCost) * Mathf.Min(dx, dy);
    }

    private void BuildPath(Dictionary<Vector3Int, Vector3Int> parent, Vector3Int start, Vector3Int goal, List<Vector3> path)
    {
        Vector3Int cell = goal;
        while (cell != start)
        {
            path.Add(CellToWorldCenter(cell));
            cell = parent[cell];
        }
        path.Reverse();
    }

    // ------------------------------------------------------------------- heap

    private class MinHeap
    {
        private struct Entry
        {
            public Vector3Int cell;
            public int f;
        }

        private readonly List<Entry> data = new List<Entry>();
        public int Count => data.Count;

        public void Push(Vector3Int cell, int f)
        {
            data.Add(new Entry { cell = cell, f = f });
            int i = data.Count - 1;
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (data[p].f <= data[i].f) break;
                (data[p], data[i]) = (data[i], data[p]);
                i = p;
            }
        }

        public Vector3Int Pop()
        {
            Vector3Int top = data[0].cell;
            int last = data.Count - 1;
            data[0] = data[last];
            data.RemoveAt(last);

            int i = 0;
            while (true)
            {
                int l = i * 2 + 1, r = l + 1, s = i;
                if (l < data.Count && data[l].f < data[s].f) s = l;
                if (r < data.Count && data[r].f < data[s].f) s = r;
                if (s == i) break;
                (data[s], data[i]) = (data[i], data[s]);
                i = s;
            }

            return top;
        }
    }

    // ----------------------------------------------------------------- gizmos

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || navTilemap == null) return;

        BoundsInt bounds = navTilemap.cellBounds;
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!navTilemap.HasTile(cell)) continue;

            bool walkable = Application.isPlaying ? IsWalkable(cell) : true;
            Gizmos.color = walkable ? new Color(0f, 1f, 0f, 0.25f) : new Color(1f, 0f, 0f, 0.35f);
            Gizmos.DrawCube(navTilemap.GetCellCenterWorld(cell), navTilemap.cellSize * 0.9f);
        }
    }
}