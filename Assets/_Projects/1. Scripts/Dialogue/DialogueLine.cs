using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[System.Serializable]
public class DialogueLine
{
    public SpeakerType speaker;
    [TextArea(2, 5)] public string text;
    public bool useAction = true;

    [Header("ACTIONS")]
    [ShowIf("useAction")] public List<DialogueAction> onShownAction = new();
    [ShowIf("useAction")] public List<DialogueAction> onAdvancedAction = new();

    [Header("EVENTS")]
    [HideIf("useAction")] public UnityEvent onShown = new();
    [HideIf("useAction")] public UnityEvent onAdvanced = new();
}