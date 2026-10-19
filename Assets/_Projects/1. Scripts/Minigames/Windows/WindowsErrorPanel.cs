using UnityEngine;
using Nova;

public class WindowsErrorPanel : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D okayBlock;
    [SerializeField] private TextBlock notification;
    [Space(10)]
    [SerializeField] private CoreButton okayButton;
    [SerializeField] private CoreButton xButton;

    private WindowsXP windowsXP;

    public enum ErrorType{
        Troll,
        Normal
    }
    ErrorType type;

    public void Init(ErrorType errorType, WindowsXP wxp){
        if(okayBlock != null) okayBlock.Position.X.Percent = Random.Range(0f, .6f);
        if(okayButton != null) okayButton.OnClick.AddListener(CallOkay);
        if(xButton != null) xButton.OnClick.AddListener(CallClose);

        type = errorType;
        windowsXP = wxp;
        string message = errorType switch
        {
            ErrorType.Troll  => "Please click Okay to spawn 5 error panels",
            ErrorType.Normal => "Please click Okay to close the error panel",
            _                => "Unknown error",
        };
        notification.Text = message;
    }

    void OnDestroy(){
        if(okayButton != null) okayButton.OnClick.RemoveListener(CallOkay);
        if(xButton != null) xButton.OnClick.RemoveListener(CallClose);
    }

    public void CallOkay(){
        if(type == ErrorType.Normal) SelfDestruct();
        else if(type == ErrorType.Troll) windowsXP.SpamError(5);
    }
    void CallClose(){
        if(type == ErrorType.Normal){
            SelfDestruct();
            windowsXP.SpamError(1);
        }
        else if(type == ErrorType.Troll) SelfDestruct();
    }
    void SelfDestruct() => Destroy(gameObject, .5f);
}