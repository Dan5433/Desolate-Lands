using CustomExtensions;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioSource playerAudio;
    [SerializeField] AudioClip[] hurtClips;

    public void PlaySoundEffect(PlayerSounds soundType)
    {
        switch (soundType)
        {
            case PlayerSounds.Hurt:
                playerAudio.PlayRandomClip(hurtClips); break;
        }
    }
}

public enum PlayerSounds
{
    Hurt = 0,
    Heal = 1,
}