using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM Sources")]
    public AudioSource bgmSource;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip playBGM;

    [Header("SFX Clips")]
    public AudioClip sfxButton;      // 일반 버튼
    public AudioClip sfxGo;          // Go 버튼 (출발)
    public AudioClip sfxReturn;      // 귀환
    public AudioClip sfxEquip;
    public AudioClip sfxHit;

    [Header("BGM Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;
    public float bgmFadeDuration = 1f;

    bool _lastIsDay;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (bgmSource != null)
        {
            bgmSource.loop   = true;
            bgmSource.volume = bgmVolume;
        }
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        _lastIsDay = DayNightManager.Instance == null || DayNightManager.Instance.IsDay;
        PlayBGM(_lastIsDay ? playBGM : playBGM);
    }

    void Update()
    {
        if (DayNightManager.Instance == null) return;
        bool isDay = DayNightManager.Instance.IsDay;
        if (isDay == _lastIsDay) return;
        _lastIsDay = isDay;
        FadeToBGM(isDay ? playBGM : playBGM);
    }

    // ── BGM ──────────────────────────────────────

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip   = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void FadeToBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        StartCoroutine(FadeBGMRoutine(clip));
    }

    public void StopBGM() => bgmSource?.Stop();

    IEnumerator FadeBGMRoutine(AudioClip nextClip)
    {
        float half = bgmFadeDuration * 0.5f;

        // 페이드 아웃
        float t = 0f;
        float startVol = bgmSource.volume;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / half);
            yield return null;
        }

        bgmSource.clip   = nextClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // 페이드 인
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / half);
            yield return null;
        }
        bgmSource.volume = bgmVolume;
    }

    // ── SFX ──────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButton()    => PlaySFX(sfxButton);
    public void PlayGo()        => PlaySFX(sfxGo);
    public void PlayReturn()    => PlaySFX(sfxReturn);
    public void PlayEquip() => PlaySFX(sfxEquip);
    public void PlayHit() => PlaySFX(sfxHit);
}
