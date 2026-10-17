using UnityEngine;

public class VolumeSliderBinder : MonoBehaviour
{
    public enum VolumeChannel { Master, Bgm, Sfx }

    [SerializeField] private CoreSlider slider;
    [SerializeField] private VolumeChannel channel;
    [SerializeField] private bool syncFromManager = true;

    void Start(){
        var sm = SettingsManager.Instance;

        if(syncFromManager && sm != null && sm.Current != null) slider.SetNormalizedValue(ReadChannel(sm.Current), notify: false);
        slider.OnValueChanged.AddListener(OnChanged);
    }

    void OnDestroy(){
        if(slider != null) slider.OnValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(float value){
        var sm = SettingsManager.Instance;
        if(sm == null) return;

        switch(channel){
            case VolumeChannel.Master: sm.SetMasterVolume(value); break;
            case VolumeChannel.Bgm:    sm.SetBgmVolume(value);    break;
            case VolumeChannel.Sfx:    sm.SetSfxVolume(value);    break;
        }
    }

    float ReadChannel(SettingsData data) => channel switch {
        VolumeChannel.Master => data.masterVolume,
        VolumeChannel.Bgm    => data.bgmVolume,
        VolumeChannel.Sfx    => data.sfxVolume,
        _ => 1f
    };
}