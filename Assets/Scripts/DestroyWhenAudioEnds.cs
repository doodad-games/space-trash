using System.Collections;
using UnityEngine;

public class DestroyWhenAudioEnds : MonoBehaviour
{
    IEnumerator Start()
    {
        var audioSource = GetComponent<AudioSource>();
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        Destroy(gameObject);
    }
}
