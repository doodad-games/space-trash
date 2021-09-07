using MyLibrary;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioClipRandomiser : MonoBehaviour
{
    public AudioClip[] audioClips;

    void OnEnable()
    {
        var source = GetComponent<AudioSource>();
        source.clip = audioClips.PickRandom();

        source.enabled = false;
        source.enabled = true;
    }
}
