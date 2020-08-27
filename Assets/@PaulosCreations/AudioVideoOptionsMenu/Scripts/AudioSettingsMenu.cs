using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsMenu : MonoBehaviour {

    public AudioMixer masterMixer;

    public Slider mainVolumeSlider, menuVolumeSlider, musicVolumeSlider, playerVolumeSlider, envVolumeSlider, enemyVolumeSlider;
    public Toggle muteToggle;

    private bool isMuted;

    // Use this for initialization
    void Start () {
        LoadMenuVariables();
	}

    public void ToggleMute(bool toggleValue)
    {
        isMuted = toggleValue;
        if (isMuted)
            masterMixer.SetFloat("mainVolume", -80);
        else
            masterMixer.SetFloat("mainVolume", Mathf.Log(mainVolumeSlider.value) * 20);
    }

    public void SetMainVolume(float sliderValue)
    {
        if (!isMuted)
            masterMixer.SetFloat("mainVolume", Mathf.Log(sliderValue) * 20);
    }

    public void SetMenuVolume(float sliderValue)
    {
        masterMixer.SetFloat("menuVolume", Mathf.Log(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        masterMixer.SetFloat("musicVolume", Mathf.Log(sliderValue) * 20);
    }
    public void SetPlayerVolume(float sliderValue)
    {
        masterMixer.SetFloat("playerVolume", Mathf.Log(sliderValue) * 20);
    }
    public void SetEnvVolume(float sliderValue)
    {
        masterMixer.SetFloat("envVolume", Mathf.Log(sliderValue) * 20);
    }
    public void SetEnemyVolume(float sliderValue)
    {
        masterMixer.SetFloat("enemyVolume", Mathf.Log(sliderValue) * 20);
    }

    public void SaveMenuVariables()
    {
        PlayerPrefs.SetInt("audioPrefsSaved", 0);

        PlayerPrefs.SetInt("mutedI", muteToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("mainVolumeF", mainVolumeSlider.value);
        PlayerPrefs.SetFloat("menuVolumeF", menuVolumeSlider.value);
        PlayerPrefs.SetFloat("musicVolumeF", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("playerVolumeF", playerVolumeSlider.value);
        PlayerPrefs.SetFloat("envVolumeF", envVolumeSlider.value);
        PlayerPrefs.SetFloat("enemyVolumeF", enemyVolumeSlider.value);
    }

    public void LoadMenuVariables()
    {
        if (PlayerPrefs.HasKey("audioPrefsSaved"))
        {
            mainVolumeSlider.value  = PlayerPrefs.GetFloat("mainVolumeF");
            menuVolumeSlider.value    = PlayerPrefs.GetFloat("menuVolumeF");
            musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolumeF");
            playerVolumeSlider.value = PlayerPrefs.GetFloat("playerVolumeF");
            envVolumeSlider.value = PlayerPrefs.GetFloat("envVolumeF");
            enemyVolumeSlider.value = PlayerPrefs.GetFloat("enemyVolumeF");

            if (PlayerPrefs.GetInt("mutedI") == 1)
                muteToggle.isOn = true;
            else muteToggle.isOn = false;
        }
    }
}
