using System.Collections;
using System.Collections.Generic;
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
        soundToggle.isOn = Global.sound;
        musicToggle.isOn = Global.music;
    }

    public void EnableSound(bool value)
    {
        Global.sound = value;
        if (Global.sound) mixer.audioMixer.SetFloat("SoundVolume", 0); //dB
        else mixer.audioMixer.SetFloat("SoundVolume", -80); //dB
    }

    public void EnableMusic(bool value)
    {
        Global.music = value;
        if (Global.music) mixer.audioMixer.SetFloat("MusicVolume", -6); //dB
        else mixer.audioMixer.SetFloat("MusicVolume", -80); //dB
    }

    public void ChangeVolume(Slider volume)
    {
        AudioListener.volume = volume.value;
    }
}
