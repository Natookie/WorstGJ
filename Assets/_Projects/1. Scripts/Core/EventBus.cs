using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public static readonly UnityEvent<string> TypeCommandEmitted = new UnityEvent<string>();
}
