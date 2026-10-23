using UnityEngine;
using System;

public enum SpeakerType
{
    MainCharacter,
    Grandma,
    StudioGuy,
    Worker, 
}

[CreateAssetMenu(fileName = "SpeakerDatabase", menuName = "Dialogue/Speaker Database")]
public class SpeakerDatabase : ScriptableObject
{
    [System.Serializable]
    public class SpeakerEntry
    {
        public string displayName;
        public Sprite displayPotrait;
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