using System;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-90)]
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public static event Action<SettingsData> OnSettingsChanged;

    private const string MasterParam = "MasterVolume";
    private const string BgmParam    = "BGMVolume";
    private const string SfxParam    = "SFXVolume";

    private const float MinDb = -80f;
    private const float MaxDb =   0f;

    [Header("MIXER")]
    [SerializeField] private AudioMixer mixer;

    [Header("DEFAULTS")]
    [SerializeField] private SettingsData defaults = new SettingsData();

    public SettingsData Current { get; private set; }

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return; 
        }
    }

    void Start(){
        var loaded = GameDataStore.Current?.settings;
        Apply(loaded ?? defaults.Clone(), notify: true);
    }

    public void SetFullscreen(bool value){
        Current.fullscreen = value;
        ApplyFullscreen(value);
        Notify();
    }

    public void SetMasterVolume(float value) { Current.masterVolume = Mathf.Clamp01(value); ApplyVolume(MasterParam, Current.masterVolume); Notify(); }
    public void SetBgmVolume(float value)    { Current.bgmVolume    = Mathf.Clamp01(value); ApplyVolume(BgmParam,    Current.bgmVolume);    Notify(); }
    public void SetSfxVolume(float value)    { Current.sfxVolume    = Mathf.Clamp01(value); ApplyVolume(SfxParam,    Current.sfxVolume);    Notify(); }

    public void Apply(SettingsData data, bool notify = true){
        Current = data ?? defaults.Clone();

        ApplyFullscreen(Current.fullscreen);
        ApplyVolume(MasterParam, Current.masterVolume);
        ApplyVolume(BgmParam,    Current.bgmVolume);
        ApplyVolume(SfxParam,    Current.sfxVolume);

        if(GameDataStore.Current != null) GameDataStore.Current.settings = Current;
        if(notify) Notify();
    }

    public void ResetToDefaults() => Apply(defaults.Clone());

    void ApplyFullscreen(bool value){
        if(value){
            var res = Screen.currentResolution;
            Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
        }
        else Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    void ApplyVolume(string mixerParam, float normalized01){
        if(mixer == null) return;

        float db = normalized01 <= 0.0001f
            ? MinDb
            : Mathf.Log10(normalized01) * 20f;

        db = Mathf.Clamp(db, MinDb, MaxDb);
        mixer.SetFloat(mixerParam, db);
    }

    void Notify() => OnSettingsChanged?.Invoke(Current);
}