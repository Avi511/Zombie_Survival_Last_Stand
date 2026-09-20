using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance{get; set;}

    [Header("Audio Sources")]
    public AudioSource musicSource;   // Background music
    public AudioSource sfxSource;     // Sound effects(Fire Bullet)(Empty Magazine)(Reload)
    public AudioSource blastSource;   //Sounds for blast/explosions (new speaker)
    public AudioSource woodShatterSource; //Sounds for shatterings
    public AudioSource zombieChannel;
    public AudioSource zombieChannel2;
    public AudioSource playerChannel;



    [Header("Background Music")]
    public AudioClip gameBGMusic;
    // Fire sound/Empty Magazine/Reload is stored in WeaponScript as a serialized field
    // Allowing each weapon prefab (M416, Sniper, Shotgun, Pistol, etc.) to have its own unique firing sound assigned through the Inspector without modifying the AudioManager or code.

    [Header("Zombie AudioClips")]
    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    [Header("Player AudioClips")]
    public AudioClip playerHurt;
    public AudioClip playerDie;
    public AudioClip gameOver;

    private void Awake()
    {
        if(Instance != null && Instance != this)        //Singleton
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (gameBGMusic == null)
        {
            return;
        }
        musicSource.clip = gameBGMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }



    public void PlayFireSound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayEmptyMagazineSound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayReloadSound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayClippingSound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }


    public void PlayBlastSound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        blastSource.PlayOneShot(clip);
    }


    public void PlayWoodShatterSound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        woodShatterSource.PlayOneShot(clip);
    }

}
