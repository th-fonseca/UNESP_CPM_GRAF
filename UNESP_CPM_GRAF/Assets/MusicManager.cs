using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    public AudioSource audioSource;
    public AudioClip mainMusic;
    public AudioClip hospitalClip;
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        // Implementação do singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = mainMusic;
        audioSource.Play();
    }

    // Função para alternar entre as músicas com fade
    public void ToggleMusicWithFade()
    {
        AudioClip newClip = audioSource.clip == mainMusic ? hospitalClip : mainMusic;
        StartCoroutine(FadeOutInMusic(newClip));
    }

    public void PlayMusicFromStart()
    {
        audioSource.Stop();
        audioSource.volume = 0.25f;
        audioSource.clip = mainMusic;
        audioSource.Play();
    }

    // Função para parar a música com fade out
    public void StopMusicWithFade()
    {
        StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutInMusic(AudioClip newClip)
    {
        yield return FadeOutMusic();
        audioSource.clip = newClip;
        audioSource.Play();
        yield return FadeInMusic();
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();
    }

    private IEnumerator FadeInMusic()
    {
        audioSource.volume = 0;
        audioSource.Play();
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0.25f;
    }
}
