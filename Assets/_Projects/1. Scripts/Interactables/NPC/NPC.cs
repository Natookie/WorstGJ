using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public bool CanInteract => true;
    public Transform Transform => transform;
    public string promptMessage => "Talk";
    [SerializeField] private DialogueAsset npcDialogue;

    public void OnHover(GameObject interactor){}
    public void OnUnhover(GameObject interactor){}
    public void OnInteract(GameObject interactor){NPCInteract();}

    public virtual void NPCInteract()
    {
        if(!DialogueManager.Instance) return;
        StartCoroutine(DialogueLibrary.Instance.Play(npcDialogue));
    }
}
