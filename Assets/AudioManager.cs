using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource bgmSource;

    public AudioClip plantSound;
    public AudioClip harvestSound;
    public AudioClip shootPistol;
    public AudioClip shootShotgun;
    public AudioClip shootRifle;
    [FormerlySerializedAs("shootSniper")]
    public AudioClip shootRocket;
    public AudioClip explosionSound;
    public AudioClip enemyHit;
    public AudioClip enemyDie;
    public AudioClip fenceHit;
    public AudioClip uiClick;
    public AudioClip buySound;
    public AudioClip unlockSound;

    public AudioClip dayBGM;
    public AudioClip nightBGM;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (bgmSource != null)
        {
            bgmSource.loop = true;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip) return;

        StartCoroutine(CrossfadeBGM(clip));
    }

    private IEnumerator CrossfadeBGM(AudioClip newClip)
    {
        const float duration = 0.5f;
        float t = 0f;

        float startVolume = bgmSource.volume;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, 0.5f, t / duration);
            yield return null;
        }

        bgmSource.volume = 0.5f;
    }

    public AudioClip GetShootSound(string weaponName)
    {
        switch (weaponName)
        {
            case "Pistol": return shootPistol;
            case "Shotgun": return shootShotgun;
            case "Rifle": return shootRifle;
            case "Rocket": return shootRocket;
            default: return null;
        }
    }
}
