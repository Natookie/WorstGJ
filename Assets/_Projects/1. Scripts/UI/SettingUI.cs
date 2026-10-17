using UnityEngine;
using Nova;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private TextBlock masterVolumeText;
    [SerializeField] private TextBlock bgmVolumeText;
    [SerializeField] private TextBlock sfxVolumeText;

    void OnEnable()  => SettingsManager.OnSettingsChanged += Handle;
    void OnDisable() => SettingsManager.OnSettingsChanged -= Handle;

    void Start(){
        var sm = SettingsManager.Instance;
        if(sm != null && sm.Current != null) Handle(sm.Current);
    }

    void Handle(SettingsData settings){
        if(settings == null) return;

        masterVolumeText.Text = $"{Mathf.RoundToInt(settings.masterVolume * 100)}%";
        bgmVolumeText.Text    = $"{Mathf.RoundToInt(settings.bgmVolume * 100)}%";
        sfxVolumeText.Text    = $"{Mathf.RoundToInt(settings.sfxVolume * 100)}%";
    }
}