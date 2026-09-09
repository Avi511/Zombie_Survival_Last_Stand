using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource reloadingSoundM16;
    public AudioSource reloadingSound1911;

    public AudioSource emptyManagizeSound1911;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;

    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    public AudioSource zombieChannel;

    public void PlayZombieSound(AudioClip clip)
    {
        if (clip == null || zombieChannel == null) return;

        zombieChannel.PlayOneShot(clip);
    }

    public void StartChaseSound()
    {
        if (zombieChase == null || zombieChannel == null) return;

        if (zombieChannel.clip != zombieChase)
        {
            zombieChannel.clip = zombieChase;
            zombieChannel.loop = true;
        }

        if (!zombieChannel.isPlaying)
        {
            zombieChannel.Play();
        }
    }

    public void StopChaseSound()
    {
        if (zombieChannel == null || zombieChannel.clip != zombieChase) return;

        zombieChannel.Stop();
        zombieChannel.loop = false;
        zombieChannel.clip = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (zombieChannel == null)
        {
            zombieChannel = gameObject.AddComponent<AudioSource>();
            zombieChannel.playOnAwake = false;
            zombieChannel.spatialBlend = 0f;
        }
    }
}