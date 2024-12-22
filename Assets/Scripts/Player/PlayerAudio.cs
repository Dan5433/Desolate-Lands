using CustomExtensions;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioSource playerAudio;
    [SerializeField] AudioClip[] hurtClips;

    public void Hurt()
    {
        playerAudio.PlayRandomClip(hurtClips);
    }
}
