using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueAsset npcDialogue;
    private bool _inConversation;

    public bool CanInteract => !_inConversation;
    public Transform Transform => transform;
    public string promptMessage => _inConversation ? string.Empty : "Talk";

    public void OnHover(GameObject interactor){}
    public void OnUnhover(GameObject interactor){}

    public void OnInteract(GameObject interactor){
        if(_inConversation) return;
        NPCInteract();
    }

    public virtual void NPCInteract(){
        if(!DialogueManager.Instance) return;
        if(npcDialogue == null) return;
        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue(){
        _inConversation = true;
        yield return DialogueLibrary.Instance.Play(npcDialogue);
        _inConversation = false;
    }
}