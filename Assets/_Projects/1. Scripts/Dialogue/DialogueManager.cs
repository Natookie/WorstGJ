using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public UnityEvent DialogueStarted;
    public UnityEvent DialogueEnded;

    public static DialogueManager Instance { get; private set; }

    [Header("REFERENCES")]
    [SerializeField] private DialogueUI dialogueUI;

    private bool isPlaying;
    private SpeakerType currentSpeaker;
    private bool advanceRequested;

    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update(){
        if(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame){
            if (dialogueUI.IsTyping()) dialogueUI.SkipTyping();
            else advanceRequested = true;
        }
    }

    public void StartDialogue() => DialogueStarted?.Invoke();
    public void EndDialogue(){
        HideDialogue();
        DialogueEnded?.Invoke();
    }

    public void PlayDialogue(SpeakerType speaker, string text){
        currentSpeaker = speaker;
        dialogueUI.ShowDialogue(speaker, text);
    }

    IEnumerator AutoHideRoutine(){
        yield return WaitForSeconds();
        HideDialogue();
    }

    public bool IsDialogueVisible() => dialogueUI.IsVisible();
    public bool IsDialogueTyping() => dialogueUI.IsTyping();

    #region API
    public IEnumerator WaitForTyping(){
        yield return new WaitUntil(() => !dialogueUI.IsTyping());
    }

    public IEnumerator WaitForSeconds(float seconds = 0.75f){
        yield return new WaitUntil(() => !dialogueUI.IsTyping());

        float timer = 0f;
        while(timer < seconds){
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator WaitForSpace(){
        yield return new WaitUntil(() => !dialogueUI.IsTyping());
        yield return new WaitUntil(() => advanceRequested);
        advanceRequested = false;
    }
    #endregion

    public void HideDialogue() => dialogueUI.HideDialogue();
    public bool IsTyping() => dialogueUI.IsTyping();
    public bool IsVisible() => dialogueUI.IsVisible();
}