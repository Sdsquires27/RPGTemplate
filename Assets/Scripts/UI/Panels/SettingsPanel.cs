using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : UIPanel
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Display")]
    [SerializeField] private Toggle fullscreenToggle;

    protected override void OnOpen()
    {
        // Load current settings
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value  = PlayerPrefs.GetFloat("MusicVolume",  1f);
        sfxVolumeSlider.value    = PlayerPrefs.GetFloat("SFXVolume",    1f);
        fullscreenToggle.isOn    = Screen.fullScreen;

        // Hook up listeners
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    protected override void OnClose()
    {
        // Save settings
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume",  musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume",    sfxVolumeSlider.value);
        PlayerPrefs.Save();

        // Remove listeners
        masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
    }

    void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    void OnMusicVolumeChanged(float value)
    {
        // TODO: hook up to your audio manager when you build one
    }

    void OnSFXVolumeChanged(float value)
    {
        // TODO: hook up to your audio manager when you build one
    }

    void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
    }
}