using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class PlayerTyping : MonoBehaviour
{
    [Serializable]
    public class InputCommand
    {
        [Tooltip("Letters only, e.g. \"jump\". Case-insensitive.")]
        public string word;
        public UnityEvent onMatched;
    }

    /// <summary>Fired for every accepted letter (always lowercase).</summary>
    public UnityEvent<char> OnLetterTyped;
    /// <summary>Fired when the buffer matches a command word (lowercase).</summary>
    public UnityEvent<string> OnCommandEmitted;

    [Header("COMMANDS")]
    public List<InputCommand> commands = new List<InputCommand>();

    [Header("BUFFER")]
    [Tooltip("Maximum letters kept in the buffer.")]
    [SerializeField] private int maxBufferLength = 16;
    [Tooltip("Seconds without typing before the buffer is cleared. 0 = never.")]
    [SerializeField] private float resetAfterSeconds = 1.5f;
    [Tooltip("Clear the buffer after a command matches.")]
    [SerializeField] private bool clearOnMatch = true;

    public string CurrentBuffer => buffer;
    public void ClearBuffer() => buffer = string.Empty;

    private string buffer = string.Empty;
    private float lastInputTime;

    private void Start()
    {
        // Call up OnCommandEmitted to EventBus.TypeCommandEmitted
        OnCommandEmitted.AddListener((string command)=>{
            EventBus.TypeCommandEmitted?.Invoke(command);});
    }

    private void OnEnable()
    {
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current != null)
                Keyboard.current.onTextInput += HandleChar;
        #endif
    }

    private void OnDisable()
    {
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current != null)
                Keyboard.current.onTextInput -= HandleChar;
        #endif
    }

    private void Update()
    {
        #if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM
            foreach (char c in Input.inputString)
                HandleChar(c);
        #endif

        if (resetAfterSeconds > 0f && buffer.Length > 0 &&
            Time.unscaledTime - lastInputTime > resetAfterSeconds)
        {
            buffer = string.Empty;
        }
    }

    private void HandleChar(char c)
    {
        // Only alphabets: ignore digits, symbols, space, backspace, enter, etc.
        if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))) return;

        c = char.ToLowerInvariant(c);
        lastInputTime = Time.unscaledTime;

        buffer += c;
        if (buffer.Length > maxBufferLength)
            buffer = buffer.Substring(buffer.Length - maxBufferLength);

        OnLetterTyped?.Invoke(c);
        CompareWithCommands();
    }

    private void CompareWithCommands()
    {
        foreach (InputCommand cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd.word)) continue;

            string target = cmd.word.ToLowerInvariant();

            // Match when the most recently typed letters equal the command word.
            if (buffer.EndsWith(target, StringComparison.Ordinal))
            {
                OnCommandEmitted?.Invoke(target);
                cmd.onMatched?.Invoke();

                if (clearOnMatch) buffer = string.Empty;
                return;
            }
        }
    }
}