using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float fadeTime = 0.05f;
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private string idleAnimPrefix = "PlayerIdle";
    [SerializeField] private string walkAnimPrefix = "PlayerWalk";

    static readonly string[] Directions = {
        "Right", "UpRight", "Up", "UpLeft", "Left", "DownLeft", "Down", "DownRight"
    };

    int lastDirection = 6;   // start facing Down
    int currentHash;

    private void Start()
    {
        Debug.Assert(animator, "animator is missing");
    }

    public void UpdateAnimation(Vector2 direction, float speed)
    {
        bool hasDirection = direction.sqrMagnitude > 0.0001f;
        bool moving = speed > moveThreshold && hasDirection;

        // Update facing whenever a direction is given, even while idle
        if (hasDirection)
            lastDirection = GetDirectionIndex(direction);

        string stateName = (moving ? walkAnimPrefix : idleAnimPrefix) + Directions[lastDirection];
        int hash = Animator.StringToHash(stateName);

        if (hash == currentHash) return;

        bool walkToWalk = moving && IsWalkState(currentHash);
        float startTime = walkToWalk ? animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f : 0f;

        currentHash = hash;
        animator.CrossFade(hash, fadeTime, 0, startTime);
    }

    private bool IsWalkState(int hash)
    {
        for (int i = 0; i < Directions.Length; i++)
            if (hash == Animator.StringToHash(walkAnimPrefix + Directions[i])) return true;
        return false;
    }

    private int GetDirectionIndex(Vector2 v)
    {
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        return Mathf.RoundToInt(angle / 45f) % 8;
    }
}

