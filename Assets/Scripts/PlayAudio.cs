using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayAudio : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] RandomAudio[] audioClipsRandom;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound(string key)
    {
        if (audioSource.isPlaying) return; 
        
        foreach(var audio in audioClipsRandom)
        {
            if(audio.key == key)
            {
                audioSource.clip = audio.clips[Random.Range(0, audio.clips.Length)];
                audioSource.Play();
            }
        }
    }
}

[Serializable]
public struct RandomAudio
{
    public string key;
    public AudioClip[] clips;
}
