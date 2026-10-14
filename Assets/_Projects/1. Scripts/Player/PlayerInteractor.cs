using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float scanInterval = 0.1f;
    [SerializeField] private InteractionPromptUI promptUI;

    private readonly Collider2D[] _buffer = new Collider2D[16];
    private IInteractable current;
    private float _timer;

    void Update(){
        _timer -= Time.deltaTime;
        if(_timer <= 0f){
            _timer = scanInterval;
            UpdateTarget();
        }

        if(current != null && Keyboard.current.eKey.wasPressedThisFrame)
            current.OnInteract(gameObject);
    }

    void UpdateTarget(){
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _buffer);

        IInteractable closest = null;
        float closestDist = float.MaxValue;

        for(int i = 0; i < count; i++){
            var hit = _buffer[i];
            if(!hit.TryGetComponent(out IInteractable interactable)) continue;
            if(!interactable.CanInteract) continue;

            float dist = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if(dist >= closestDist) continue;

            closestDist = dist;
            closest = interactable;
        }

        if(ReferenceEquals(closest, current)) return;

        current = closest;
        Refresh();
    }

    public void Refresh() => promptUI?.Show(current?.promptMessage);
}