using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Asset", fileName = "Dialogue_")]
public class DialogueAsset : ScriptableObject
{
    [SerializeField] private List<DialogueLine> lines = new();
    public IReadOnlyList<DialogueLine> Lines => lines ??= new List<DialogueLine>();
}