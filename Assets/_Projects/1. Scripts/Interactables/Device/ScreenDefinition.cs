using UnityEngine;

[CreateAssetMenu(menuName = "Game/Screen Definition", fileName = "Screen_")]
public class ScreenDefinition : ScriptableObject
{
    [SerializeField] private string id;
    public string Id => string.IsNullOrEmpty(id) ? name : id;
}