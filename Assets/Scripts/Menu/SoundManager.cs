using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; 

    public AudioSource[] sfxSources; // 효과음 오디오 소스 배열
    private float sfxVolume = 1.0f; // 효과음 기본 볼륨

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        ApplySFXVolume();
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxSources.Length)
        {
            sfxSources[index].volume = sfxVolume;
            sfxSources[index].Play();
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        ApplySFXVolume();
    }

    private void ApplySFXVolume()
    {
        foreach (var source in sfxSources)
        {
            source.volume = sfxVolume;
        }
    }
}
