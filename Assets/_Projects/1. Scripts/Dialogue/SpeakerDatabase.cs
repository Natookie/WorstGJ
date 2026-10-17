using UnityEngine;
using System;

public enum SpeakerType
{
    Orang1,
    Orang2
}

[CreateAssetMenu(fileName = "SpeakerDatabase", menuName = "Dialogue/Speaker Database")]
public class SpeakerDatabase : ScriptableObject
{
    [System.Serializable]
    public class SpeakerEntry
    {
        public string displayName;
        public SpeakerType type;
        public Color textColor = Color.white;
    }

    [SerializeField] private SpeakerEntry[] speakers;

    public SpeakerEntry GetSpeaker(SpeakerType type){
        foreach(var speaker in speakers)
            if(speaker.type == type) return speaker;
        return null;
    }
}