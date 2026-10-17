using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Door targetDoor;
    [SerializeField] private string label = "Door";

    public enum EntryDirection { Left, Right, Up, Down }
    public EntryDirection entryDirection = EntryDirection.Up;

    public string Id => gameObject.name;
    public Door TargetId => targetDoor;

    public bool CanInteract => true;
    public Transform Transform => transform;
    public string promptMessage => label;

    public void OnHover(GameObject interactor) {}
    
    public void OnUnhover(GameObject interactor) {}

    public void OnInteract(GameObject interactor) => RoomManager.Instance.Travel(targetDoor, entryDirection);
}