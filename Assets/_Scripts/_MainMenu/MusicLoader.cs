using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoader : MonoBehaviour
{
    public static MusicLoader Instance { get; private set; }
    private SoundManager sounds;
    public AudioClip clip;
    public AudioSource source;

    [Header("Fade In")]
    public float fadeInDuration;

    [Header("Fade Out")]
    private bool measureVolume = false;
    public float fadeOutDuration = 1f;


    void Awake()
    {
        Instance = this;
    }

    public void InstantiateStart()
    {
        sounds = SoundManager.Instance;
        source.clip = clip;
        source.volume = 0;
        source.Play();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (!measureVolume)
        {
            return;
        }
        source.volume = sounds.backgroundVolume;
    }

    public void CallFadeOut()
    {
        measureVolume = false;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        float duration = fadeOutDuration;
        float backgroundVolume = sounds.backgroundVolume;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(backgroundVolume, 0, t / duration);
            yield return null;
        }

        source.Stop();
    }
    
    IEnumerator FadeIn()
    {
        float t = 0;
        float duration = fadeOutDuration;
        float backgroundVolume = sounds.backgroundVolume;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0, backgroundVolume, t / duration);
            yield return null;
        }

        measureVolume = true;
    }
}
