using UnityEngine;

/// <summary>
/// Global music + SFX manager. Lives through scene loads.
/// - Call AudioManager.I.PlayMusic(clip) to fade to new track
/// - Volume is saved in PlayerPrefs ("music_vol")
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    [Header("Assign in Inspector")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    AudioSource sfxSource;
    AudioSource musicSource;

    const string VOL_KEY = "music_vol";
    float _targetVol = 0.6f;                 // default
    public float Volume => musicSource != null ? musicSource.volume : _targetVol;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // load saved volume (default 0.6)
        _targetVol = PlayerPrefs.GetFloat(VOL_KEY, 0.6f);
        musicSource.volume = 0f;             // fades up to target
    }

    public void PlaySFX(AudioClip c) { if (c) sfxSource.PlayOneShot(c); }

    public void PlayMusic(AudioClip clip, float fadeSeconds = 1f)
    {
        if (!clip) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(clip, fadeSeconds));
    }

    System.Collections.IEnumerator FadeTo(AudioClip next, float fade)
    {
        // fade out current (use unscaled so it works while game paused)
        float t = 0f; float start = musicSource.volume;
        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, 0f, t / fade);
            yield return null;
        }

        musicSource.clip = next;
        musicSource.Play();

        // fade in to target volume
        t = 0f;
        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, _targetVol, t / fade);
            yield return null;
        }
        musicSource.volume = _targetVol;
    }

    public void SetMusicVolume(float v)
    {
        _targetVol = Mathf.Clamp01(v);
        if (musicSource) musicSource.volume = _targetVol;
        PlayerPrefs.SetFloat(VOL_KEY, _targetVol);
    }
}
