using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Nova;

public class StudioPanel : MonoBehaviour
{
    [Header("INPUT")]
    [SerializeField] private CoreButton submitButton;
    [SerializeField] private CoreTextField textField;

    [Header("QR")]
    [SerializeField] private UIBlock2D qrBlock;
    [SerializeField] private float qrSpeed = 0.2f;

    [Header("FEEDBACK")]
    [SerializeField] private TextBlock feedbackText;

    private static readonly string[] VALID_IMAGE = {
        "id1", "id2", "id3", "id4", "id5", "id6"
    };

    private float _minX, _maxX, _minY, _maxY;
    private float _vx, _vy;
    private bool _initialized;

    void Start(){
        if(submitButton != null) submitButton.OnClick.AddListener(OnSubmit);
        InitQr();
    }

    void OnDestroy(){
        if(submitButton != null) submitButton.OnClick.RemoveListener(OnSubmit);
    }

    void InitQr(){
        if(qrBlock == null) return;

        float sizeX = qrBlock.Size.X.Percent;
        float sizeY = qrBlock.Size.Y.Percent;

        _minX = 0f;
        _maxX = 1f - sizeX;
        _minY = 0f;
        _maxY = 1f - sizeY;

        float startX = UnityEngine.Random.Range(_minX, _maxX);
        float startY = UnityEngine.Random.Range(_minY, _maxY);
        qrBlock.Position.X.Percent = startX;
        qrBlock.Position.Y.Percent = startY;

        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        _vx = Mathf.Cos(angle) * qrSpeed;
        _vy = Mathf.Sin(angle) * qrSpeed;

        _initialized = true;
        Debug.Log($"[QR Init] sizeX={qrBlock.Size.X.Percent}, sizeY={qrBlock.Size.Y.Percent}, maxX={_maxX}, maxY={_maxY}");
    }

    void Update(){
        if(!_initialized) return;
        if(!qrBlock.gameObject.activeSelf) return;

        float dt = Time.deltaTime;

        float x = qrBlock.Position.X.Percent + _vx * dt;
        float y = qrBlock.Position.Y.Percent + _vy * dt;

        if(x <= _minX){ x = _minX; _vx = -_vx; }
        else if(x >= _maxX){ x = _maxX; _vx = -_vx; }

        if(y <= _minY){ y = _minY; _vy = -_vy; }
        else if(y >= _maxY){ y = _maxY; _vy = -_vy; }

        qrBlock.Position.X.Percent = x;
        qrBlock.Position.Y.Percent = y;
    }

    void OnSubmit(){
        if(textField == null) return;

        string raw = textField.Text ?? string.Empty;
        if(string.IsNullOrWhiteSpace(raw)) return;

        var validSet = new HashSet<string>(VALID_IMAGE, StringComparer.OrdinalIgnoreCase);
        var submittedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        string[] tokens = raw.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach(var token in tokens){
            string trimmed = token.Trim();
            if(string.IsNullOrEmpty(trimmed)) continue;
            submittedSet.Add(trimmed);
        }

        if(submittedSet.Count == 0) return;

        var correct = new List<string>();
        foreach(var item in submittedSet)
            if(validSet.Contains(item)) correct.Add(item);

        int totalValid = VALID_IMAGE.Length;
        bool allFound = correct.Count == totalValid;

        string message;
        if(correct.Count == 0) message = "None is correct.";
        else if(allFound) message = $"{FormatList(correct)} is correct. All found!";
        else message = $"{FormatList(correct)} is correct, but it's still not fully correct.";

        if(feedbackText != null){
            feedbackText.Text = message;
            feedbackText.Visible = true;
        }

        textField.SetText(string.Empty);
    }

    private static string FormatList(List<string> items){
        if(items.Count == 0) return string.Empty;
        if(items.Count == 1) return items[0];
        if(items.Count == 2) return $"{items[0]} and {items[1]}";

        var sb = new StringBuilder();
        for(int i = 0; i < items.Count; i++){
            if(i > 0) sb.Append(", ");
            if(i == items.Count - 1) sb.Append("and ");
            sb.Append(items[i]);
        }
        return sb.ToString();
    }
}