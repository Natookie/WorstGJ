using UnityEngine;
using Nova;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private TextBlock label;

    void Awake(){
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Show(string message){
        if(string.IsNullOrEmpty(message)){
            root.SetActive(false);
            return;
        }

        label.Text = message;
        root.SetActive(true);
    }

    public void Hide() => root.SetActive(false);
}