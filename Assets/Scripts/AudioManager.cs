using UnityEngine;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip paddleHit;
    public AudioClip blockDestroy;
    public AudioClip powerUp;
    public AudioClip gameOver;

     public AudioClip victory;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayVictorySound()
    {
        Debug.Log("Attempting to play victory sound...");
        if (victory != null && sfxSource != null)
        {
            Debug.Log("Playing victory sound!");
            sfxSource.PlayOneShot(victory);
            // Opcional: Detener la música actual
            StartCoroutine(FadeOutMusic(1f));
        }
        else
        {
            Debug.LogError($"Victory sound setup incomplete! victory clip: {(victory == null ? "null" : "ok")}, sfxSource: {(sfxSource == null ? "null" : "ok")}");
        }
    }

    public void PlayGameOverSound()
    {
        if (gameOver != null)
        {
            PlaySFX(gameOver);
            // Opcional: Detener la música actual
            StopMusic();
        }
        else
        {
            Debug.LogWarning("Game Over sound clip not assigned!");
        }
    }

     private IEnumerator FadeOutMusic(float duration)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = startVolume;
        }
    }

    public void PlayMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }


}
