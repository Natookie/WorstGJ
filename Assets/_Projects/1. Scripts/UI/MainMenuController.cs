using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject root;

    void OnEnable()  => GameManager.OnStateChanged += Handle;
    void OnDisable() => GameManager.OnStateChanged -= Handle;

    void Start(){
        if(GameManager.Instance != null) Handle(GameManager.Instance.State);
    }

    void Handle(GameManager.GameState state) => root.SetActive(state == GameManager.GameState.Menu);
}