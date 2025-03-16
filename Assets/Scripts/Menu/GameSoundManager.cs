using UnityEngine;
using System.Collections.Generic;
using UltimateClean;

public class GameSoundManager : MonoBehaviour
{
    public static GameSoundManager instance;
    public List<AudioSource> bgmSourcesList = new List<AudioSource>();

    private float bgmVolume = 1.0f; 
    private float sfxVolume = 1.0f;

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
            return;
        }

        LoadSoundSettings();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();

        foreach (var bgm in bgmSourcesList)
        {
            if (bgm != null && bgm.clip != null && bgm.clip.name.StartsWith("M_"))
            {
                bgm.volume = bgmVolume;
            }
            else
            {
                Debug.Log($"X: {bgm.clip?.name}");
            }

        }

        Debug.Log($"[GameSoundManager] BGM 볼륨 변경: {bgmVolume}");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("sound_on", sfxVolume);
        PlayerPrefs.Save();

        UltimateClean.SoundManager[] soundManagers = FindObjectsOfType<UltimateClean.SoundManager>();
        foreach (var sm in soundManagers)
        {
            sm.SwitchSound();
        }

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var source in audioSources)
        {
            if (source.clip != null && source.clip.name.StartsWith("S_"))
            {
                source.volume = sfxVolume;
            }
        }

        ButtonSounds[] buttonSounds = FindObjectsOfType<ButtonSounds>();
        foreach (var buttonSound in buttonSounds)
        {
            buttonSound.UpdateButtonSoundVolume(sfxVolume);
        }

        Debug.Log($"[GameSoundManager] SFX 볼륨 변경: {sfxVolume}");
    }



    private void LoadSoundSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("sound_on", 1.0f);

        ApplyBGMVolume();
        ApplySFXVolume();
    }

    private void ApplyBGMVolume()
    {
        foreach (var bgm in bgmSourcesList)
        {
            if (bgm != null)
            {
                bgm.volume = bgmVolume;
            }
        }
    }

    private void ApplySFXVolume()
    {
        AudioListener.volume = sfxVolume;
    }

    public void RefreshBGMSources()
    {
        bgmSourcesList.Clear();
        bgmSourcesList.AddRange(FindObjectsOfType<AudioSource>());
        ApplyBGMVolume();
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}

