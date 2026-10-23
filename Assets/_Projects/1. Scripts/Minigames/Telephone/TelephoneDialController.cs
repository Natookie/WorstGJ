using UnityEngine;

public class TelephoneDialController : MonoBehaviour
{
    [SerializeField] private TelephonePanel panel;
    [SerializeField] private CoreButton dialButton;
    [SerializeField] private CoreButton resetButton;
    [Space(10)]
    [SerializeField] private DialogueAsset successTelephoneDialogue;
    [SerializeField] private DialogueAsset failedTelephoneDialogue;
    int currentIndex = 0;

    private const string DIAL_TARGET = "4827316";
    private string _dialedNumber = "";

    void Awake(){
        if(dialButton != null) dialButton.OnClick.AddListener(OnDialPressed);
        if(resetButton != null) resetButton.OnClick.AddListener(OnDialReset);
    }

    void OnDestroy(){
        if(dialButton != null) dialButton.OnClick.RemoveListener(OnDialPressed);
        if(resetButton != null) resetButton.OnClick.RemoveListener(OnDialReset);
    }

    void OnDialPressed(){
        if(panel == null) return;

        string number = panel.GetClosestChildText();
        if(string.IsNullOrEmpty(number)) return;

        _dialedNumber += number;
        if(currentIndex >= panel.GetNumberBlockChildCount() - 1) OnDialReset();

        panel.SetNumberBlock(currentIndex++, number);
        panel.RandomizePointer();
    }
    void OnDialReset(){
        if(_dialedNumber.Equals(DIAL_TARGET)) StartCoroutine(DialogueLibrary.Instance.Play(successTelephoneDialogue));
        else StartCoroutine(DialogueLibrary.Instance.Play(failedTelephoneDialogue));

        panel.ResetNumberBlock();
        currentIndex = 0;
    }

    public string GetDialedNumber() => _dialedNumber;
    public void ClearNumber() => _dialedNumber = "";
}