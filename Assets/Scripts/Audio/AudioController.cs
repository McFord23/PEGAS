using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioMixerGroup mixer;
    public Slider volume;
    public Toggle soundToggle;
    public Toggle musicToggle;

    void Start()
    {
        volume.value = AudioListener.volume;
        soundToggle.isOn = Settings.Sound;
        musicToggle.isOn = Settings.Music;
    }

    public void EnableSound(bool value)
    {
        Settings.Sound = value;
        if (Settings.Sound) mixer.audioMixer.SetFloat("SoundVolume", 0); //dB
        else mixer.audioMixer.SetFloat("SoundVolume", -80); //dB
    }

    public void EnableMusic(bool value)
    {
        Settings.Music = value;
        if (Settings.Music) mixer.audioMixer.SetFloat("MusicVolume", -6); //dB
        else mixer.audioMixer.SetFloat("MusicVolume", -80); //dB
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }
}
