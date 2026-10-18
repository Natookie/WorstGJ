using UnityEngine;

[CreateAssetMenu(menuName = "Events/Button Click Event")]
public class ButtonClickChannel : ScriptableObject
{
    public event System.Action<string> OnRaised;
    public void Raise(string id) => OnRaised?.Invoke(id);
}