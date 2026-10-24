using UnityEngine;

public class NPCRoam : NPC
{
    [Header("REFERENCES")]
    [SerializeField] private NPCMovementComponent npcMovementComponent;
    [SerializeField] private NPCVisual npcVisual;
    [Header("NPC SETTINGS")]
    [SerializeField] private float minIdleTime = 0.5f;
    [SerializeField] private float maxIdleTime = 2.0f;

    // Idle
    private bool isIdle = false;
    private float idleTimer;
    // Walk
    private bool isWalking = false;
    private Vector3 walkTargetPosition;
    // Talk
    private bool isTalking = false;

    private void Start()
    {
        Debug.Assert(npcMovementComponent, "npcMovementComponent is missing");
        Debug.Assert(npcVisual, "npcVisual is missing");

        if (DialogueManager.Instance)
        {
            DialogueManager.Instance.DialogueEnded.AddListener(OnDialogueEnded);
        }
        npcMovementComponent.MovePointReached.AddListener(OnMovePointReached);

        DoIdle();
    }

    private void Update()
    {
        if (isTalking) return;

        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0.0f) DoWalk();
        }

        if (npcVisual)
        {
            npcVisual.UpdateAnimation(
                npcMovementComponent.Velocity.normalized,
                npcMovementComponent.Velocity.sqrMagnitude
            );
        }
    }

    public override void NPCInteract()
    {
        if (!DialogueManager.Instance) return;

        DoTalk();
        //DialogueLibrary.Instance.StartCoroutine(DialogueLibrary.Instance.TalkWithNPC());
    }

    private void DoIdle()
    {
        isIdle = true;
        isWalking = false;

        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void DoWalk()
    {
        isIdle = false;
        isWalking = true;
        
        if (npcMovementComponent.TilemapNav)
        {
            if (npcMovementComponent.TilemapNav.TryGetRandomWalkablePosition(out walkTargetPosition))
                npcMovementComponent.MoveTo(walkTargetPosition);
            else Debug.Log("TilemapNav cannot find walkable position");
        }
    }

    private void DoTalk()
    {
        isTalking = true;
        
        npcMovementComponent.StopMove();
    }

    private void OnDialogueEnded()
    {
        isTalking = false;

        // Continue walking
        if (isWalking) npcMovementComponent.MoveTo(walkTargetPosition);
    }

    private void OnMovePointReached(Vector3 Destination) {DoIdle();}
}
