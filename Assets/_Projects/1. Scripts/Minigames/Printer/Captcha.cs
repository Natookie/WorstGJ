using System.Collections.Generic;
using UnityEngine;
using Nova;
using System.Collections;

public class Captcha : MonoBehaviour
{
    [Header("ROOT")]
    [SerializeField] private GameObject robotRoot;
    [SerializeField] private GameObject imageGridRoot;
    [SerializeField] private GameObject textMatchRoot;

    [Header("ROBOT")]
    [SerializeField] private CoreButton notRobotButton;
    [SerializeField] private Sprite checkmark;

    [Header("IMAGE GRID")]
    [SerializeField] private CoreButton[] imageGrids;
    [SerializeField] private CoreButton imageSubmitButton;
    [SerializeField] private string skipLabel = "Skip";
    [SerializeField] private string submitLabel = "Submit";
    [SerializeField] private int decoyOffsetRange = 5;

    [Header("TEXT MATCH")]
    [SerializeField] private TextBlock textMatchSequence;
    [SerializeField] private CoreTextField textMatchField;
    [SerializeField] private CoreButton textMatchSubmitButton;
    [SerializeField] private int textMatchMinLength = 10;
    [SerializeField] private int textMatchMaxLength = 25;

    int[] Fib = new int[25]{
        0, 1, 1, 2, 3, 5, 8, 13, 21, 34,
        55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181,
        6765, 10946, 17711, 28657, 46368
    };

    private const int IMAGE_COUNT = 9;
    private const string TextMatchGroup = "textmatch";

    private int[] currentSequence;
    private bool[] selectedStates;
    private string currentTextSequence;

    void Awake(){
        selectedStates = new bool[IMAGE_COUNT];

        for(int i = 0; i < imageGrids.Length; i++){
            if(imageGrids[i] == null) continue;
            int capturedIndex = i;
            imageGrids[i].OnClick.AddListener(() => ToggleImage(capturedIndex));
        }

        notRobotButton?.OnClick.AddListener(ImNotRobotClicked);
        imageSubmitButton?.OnClick.AddListener(OnImageSubmitClicked);
        textMatchSubmitButton?.OnClick.AddListener(OnTextMatchSubmitClicked);
    }

    void OnDestroy(){
        for(int i = 0; i < imageGrids.Length; i++){
            if(imageGrids[i] == null) continue;
            int capturedIndex = i;
            imageGrids[i].OnClick.RemoveListener(() => ToggleImage(capturedIndex));
        }

        imageSubmitButton?.OnClick.RemoveListener(OnImageSubmitClicked);
        textMatchSubmitButton?.OnClick.RemoveListener(OnTextMatchSubmitClicked);
    }

    #region ROBOT
    public void ToggleRobotPanel() => robotRoot.SetActive(!robotRoot.activeSelf);
    public void ImNotRobotClicked(){
        notRobotButton.SetSelected(true);
        notRobotButton.SetIcon(checkmark);
        StartCoroutine(ImNotRobotClickedDelayed());
    }
    IEnumerator ImNotRobotClickedDelayed(){
        yield return new WaitForSeconds(1.2f);
        ToggleRobotPanel();
        ToggleImageGridPanel();
    }
    #endregion

    #region IMAGE GRID
    public void ToggleImageGridPanel(){
        imageGridRoot.SetActive(!imageGridRoot.activeSelf);
        GenerateFibbonaciSequence();
    }

    public void GenerateFibbonaciSequence(){
        int fibonacciN = Random.Range(2, 5);
        int startIndex = Random.Range(2, Fib.Length - fibonacciN);

        currentSequence = new int[fibonacciN];
        for(int i = 0; i < fibonacciN; i++) currentSequence[i] = Fib[startIndex + i];

        int[] slotIndices = new int[IMAGE_COUNT];
        for(int i = 0; i < IMAGE_COUNT; i++) slotIndices[i] = i;
        for(int i = IMAGE_COUNT - 1; i > 0; i--){
            int j = Random.Range(0, i + 1);
            (slotIndices[i], slotIndices[j]) = (slotIndices[j], slotIndices[i]);
        }

        int[] assigned = new int[IMAGE_COUNT];
        for(int i = 0; i < fibonacciN; i++){
            assigned[slotIndices[i]] = currentSequence[i];
            Debug.Log(currentSequence[i]);
        }

        for(int i = fibonacciN; i < IMAGE_COUNT; i++){
            int decoy;
            bool invalid;
            do{
                int baseFib = Fib[Random.Range(1, Fib.Length)];
                int offset = Random.Range(-decoyOffsetRange, decoyOffsetRange + 1);
                decoy = Mathf.Max(1, baseFib + offset);
                invalid = false;

                for(int f = 1; f < Fib.Length; f++){
                    if(Fib[f] == decoy){
                        invalid = true;
                        break;
                    }
                }

                if(!invalid){
                    for(int k = fibonacciN; k < i; k++){
                        if(assigned[slotIndices[k]] == decoy){
                            invalid = true;
                            break;
                        }
                    }
                }
            } while(invalid);

            assigned[slotIndices[i]] = decoy;
        }

        for(int i = 0; i < IMAGE_COUNT; i++){
            imageGrids[i].SetText(assigned[i].ToString());
            imageGrids[i].SetSelected(false);
            selectedStates[i] = false;
        }

        UpdateSubmitLabel();
    }

    void ToggleImage(int index){
        if(index < 0 || index >= IMAGE_COUNT) return;
        if(imageGrids[index] == null) return;

        selectedStates[index] = !selectedStates[index];
        imageGrids[index].SetSelected(selectedStates[index]);

        UpdateSubmitLabel();
    }

    int SelectedCount(){
        int count = 0;
        for(int i = 0; i < selectedStates.Length; i++)
            if(selectedStates[i]) count++;
        return count;
    }

    void UpdateSubmitLabel(){
        if(imageSubmitButton == null) return;
        imageSubmitButton.SetText(SelectedCount() == 0 ? skipLabel : submitLabel);
    }

    void OnImageSubmitClicked(){
        if(SelectedCount() == 0){
            Debug.Log("[Captcha] Image grid skipped");
            return;
        }

        var selectedValues = new List<int>();
        for(int i = 0; i < IMAGE_COUNT; i++){
            if(!selectedStates[i]) continue;
            if(int.TryParse(imageGrids[i].GetText(), out int value))
                selectedValues.Add(value);
        }

        bool valid = ValidateSelection(selectedValues);
        if(!valid) GenerateFibbonaciSequence();
        else ToggleTextMatchPanel();
    }

    bool ValidateSelection(List<int> selected){
        if(currentSequence == null || currentSequence.Length == 0) return false;
        if(selected.Count != currentSequence.Length) return false;

        var a = new HashSet<int>(selected);
        var b = new HashSet<int>(currentSequence);
        return a.SetEquals(b);
    }
    #endregion

    #region TEXT MATCH
    public void ToggleTextMatchPanel(){
        ToggleImageGridPanel();
        textMatchRoot.SetActive(!textMatchRoot.activeSelf);
        GenerateTextSequence();
    }

    void GenerateTextSequence(){
        int length = Random.Range(textMatchMinLength, textMatchMaxLength + 1);

        var chars = new char[length];
        for(int i = 0; i < length; i++) chars[i] = Random.value < 0.5f ? 'm' : 'n';

        currentTextSequence = new string(chars);

        if(textMatchSequence != null) textMatchSequence.Text = currentTextSequence;
        if(textMatchField != null){
            textMatchField.GroupId = TextMatchGroup;
            textMatchField.Validator = s => s == currentTextSequence;

            textMatchField.SetText(string.Empty);
            TextFieldManager.ClearAll(TextMatchGroup);
        }
    }

    void OnTextMatchSubmitClicked(){
        if(textMatchField == null) return;

        if(string.IsNullOrEmpty(textMatchField.Text)) return;
        if(textMatchField.IsValid()) Debug.Log("[Captcha] Text match passed");
        else GenerateTextSequence();
    }
    #endregion

    int GetFib(int index){
        if(index < 0 || index >= Fib.Length) return 0;
        return Fib[index];
    }
}