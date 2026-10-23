using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Play Sound")]
public class PlaySoundAction : DialogueAction
{
    [SerializeField] private AudioClip clip;

    public override void Execute(){
        if(clip != null) AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}