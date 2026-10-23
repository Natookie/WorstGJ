using UnityEngine;
using Nova;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private TextBlock label;

    private bool _suppressed;

    void Awake(){
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start(){
        if(CameraController.Instance != null){
            CameraController.Instance.OnScreenModeEnter += HandleScreenModeEnter;
            CameraController.Instance.OnScreenModeExit += HandleScreenModeExit;
            _suppressed = CameraController.Instance.IsInScreenMode;
            if(_suppressed) Hide();
        }
    }

    void OnDisable(){
        if(CameraController.Instance != null){
            CameraController.Instance.OnScreenModeEnter -= HandleScreenModeEnter;
            CameraController.Instance.OnScreenModeExit -= HandleScreenModeExit;
        }
    }

    void HandleScreenModeEnter(){
        _suppressed = true;
        Hide();
    }
    void HandleScreenModeExit() => _suppressed = false;

    public void Show(string message){
        if(_suppressed) return;
        if(string.IsNullOrEmpty(message)){
            root.SetActive(false);
            return;
        }

        label.Text = message;
        root.SetActive(true);
    }

    public void Hide() => root.SetActive(false);
}