using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Nova;

[RequireComponent(typeof(Nova.Interactable))]
public class CoreTextField : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D backgroundBlock;
    [SerializeField] private TextBlock inputText;
    [SerializeField] private TextBlock placeholderText;
    public string GroupId;

    [Header("VISUALS")]
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 focusedColor;
    [SerializeField] private Color32 invalidColor;

    [Header("SETTINGS")]
    [SerializeField] private int maxLength = 32;
    [SerializeField] private bool blockBackspace = true;

    [Header("VALIDATION")]
    [SerializeField] private bool required = false;
    [SerializeField] private int minLength = 0;

    public event Action<string> OnChanged;
    public event Action<string> OnSubmit;

    private string _text = "";
    private bool _focused;
    private Func<string, bool> _customValidator;
    private TextFieldValidator[] _validators;

    public bool IsFocused => _focused;
    public string Text => _text;

    public Func<string, bool> Validator {
        get => _customValidator;
        set => _customValidator = value;
    }

    public bool IsValid(out string[] failedRules){
        var failed = new List<string>();

        if(required && string.IsNullOrEmpty(_text)) failed.Add("Required");
        if(_text.Length < minLength) failed.Add($"MinLength({minLength})");
        if(_customValidator != null && !_customValidator(_text)) failed.Add("CustomRule");

        if(_validators != null){
            foreach(var v in _validators){
                if(v != null && !v.Validate(_text))
                    failed.Add(v.GetRuleName());
            }
        }

        failedRules = failed.ToArray();
        return failed.Count == 0;
    }

    public bool IsValid() => IsValid(out _);

    public void SetText(string value){
        _text = value ?? "";
        if(_text.Length > maxLength) _text = _text.Substring(0, maxLength);
        RefreshVisuals();
        OnChanged?.Invoke(_text);
    }

    void Awake(){
        _validators = GetComponents<TextFieldValidator>();

        var block = backgroundBlock != null ? backgroundBlock : GetComponent<UIBlock2D>();
        block.AddGestureHandler<Gesture.OnPress>(_ => TextFieldManager.RequestFocus(this));
    }

    void OnEnable(){
        TextFieldManager.Register(this);
        if(Keyboard.current != null)
            Keyboard.current.onTextInput += HandleTextInput;
        RefreshVisuals();
    }

    void OnDisable(){
        TextFieldManager.Unregister(this);
        if(Keyboard.current != null)
            Keyboard.current.onTextInput -= HandleTextInput;
    }

    public void Focus(){
        _focused = true;
        RefreshVisuals();
    }

    public void Defocus(){
        _focused = false;
        RefreshVisuals();
    }

    public void Clear(){
        _text = "";
        RefreshVisuals();
        OnChanged?.Invoke(_text);
    }

    public void Submit(){
        OnSubmit?.Invoke(_text);
    }

    public void MarkInvalid(){
        if(backgroundBlock == null) return;
        backgroundBlock.Color = invalidColor;
        CancelInvoke(nameof(RestoreColor));
        Invoke(nameof(RestoreColor), 0.4f);
    }

    void RestoreColor(){
        RefreshVisuals();
    }

    void Update(){
        if(!_focused) return;
        if(Keyboard.current == null) return;

        if(Keyboard.current.backspaceKey.wasPressedThisFrame){
            if(blockBackspace) return;

            if(_text.Length > 0){
                _text = _text.Substring(0, _text.Length - 1);
                RefreshVisuals();
                OnChanged?.Invoke(_text);
            }
            return;
        }

        if(Keyboard.current.enterKey.wasPressedThisFrame) OnSubmit?.Invoke(_text);
    }

    void HandleTextInput(char c){
        if(!_focused) return;
        if(char.IsControl(c)) return;
        if(_text.Length >= maxLength) return;

        _text += c;
        RefreshVisuals();
        OnChanged?.Invoke(_text);
    }

    void RefreshVisuals(){
        if(backgroundBlock != null) backgroundBlock.Color = _focused ? focusedColor : normalColor;
        if(inputText != null) inputText.Text = _text;
        if(placeholderText != null) placeholderText.Visible = string.IsNullOrEmpty(_text);
    }
}