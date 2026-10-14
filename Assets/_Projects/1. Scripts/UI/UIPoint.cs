using UnityEngine;

public class UIPoint : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float yOffset;

    void Update(){
        if(playerTarget != null) transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y + yOffset, transform.position.z);
    }
}