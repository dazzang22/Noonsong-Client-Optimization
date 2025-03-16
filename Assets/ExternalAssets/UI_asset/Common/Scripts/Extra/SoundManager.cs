// Copyright (C) 2015-2021 gamevanilla - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace UltimateClean
{
    /// <summary>
    /// This class handles updating the sound UI widgets depending on the player's selection.
    /// </summary>
    //public class SoundManager : MonoBehaviour
    //{
    //    private Slider m_soundSlider;
    //    private GameObject m_soundButton;

    //    private void Start()
    //    {
    //        m_soundSlider = GetComponent<Slider>();
    //        m_soundSlider.value = PlayerPrefs.GetInt("sound_on");
    //        m_soundButton = GameObject.Find("SoundButton/Button");
    //    }

    //    public void SwitchSound()
    //    {
    //        AudioListener.volume = m_soundSlider.value;
    //        PlayerPrefs.SetInt("sound_on", (int)m_soundSlider.value);
    //        if (m_soundButton != null)
    //        {
    //            m_soundButton.GetComponent<SoundButton>().ToggleSprite();
    //        }
    //    }
    //}

    public class SoundManager : MonoBehaviour
    {
        private Slider m_soundSlider;
        private GameObject m_soundButton;
        private float sfxVolume = 1f;

        private void Start()
        {
            m_soundSlider = GetComponent<Slider>();
            sfxVolume = PlayerPrefs.GetFloat("sound_on", 1f);
            m_soundSlider.value = sfxVolume;
            m_soundButton = GameObject.Find("SoundButton/Button");

            ApplySoundVolume();
        }

        public void SwitchSound()
        {
            sfxVolume = m_soundSlider.value;
            PlayerPrefs.SetFloat("sound_on", sfxVolume);
            PlayerPrefs.Save();

            ApplySoundVolume(); 
        }

        private void ApplySoundVolume()
        {
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
        }
    }
}
