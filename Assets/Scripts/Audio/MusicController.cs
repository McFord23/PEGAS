using System;
using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Serializable]
    public class MusicTheme
    {
        public Music main;
        public Music pause;
        public Music victory;
    }

    [Serializable]
    public class Music
    {
        public bool hasIntro;
        public AudioClip intro;
        public AudioClip loop;
    }

    public MusicTheme celestiaTheme;
    public MusicTheme lunaTheme;
    public MusicTheme coopTheme;
    private MusicTheme currentTheme;
    
    private AudioSource musicPlayer;
    private Coroutine playIntro;
    private bool savedLoop;
    private float savedTime;

    private void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        Settings.OnChangeGameModeEvent += ChangeTheme;
        PlayersSettings.OnSwapCharactersEvent += ChangeTheme;
        
        ChangeTheme();
        PlayMainMusic();
    }
    
    public void PauseMusic()
    {
        savedTime = musicPlayer.time;
        savedLoop = musicPlayer.loop;
        musicPlayer.Stop();

        PlayMusic(currentTheme.pause);
    }

    public void ResumeMusic()
    {
        if (playIntro != null)
        {
            StopCoroutine(playIntro);
            playIntro = null;
        }
        
        musicPlayer.Stop();
        
        if (savedLoop)
        {
            musicPlayer.clip = currentTheme.main.loop;
            musicPlayer.time = savedTime;
            musicPlayer.loop = savedLoop;
            musicPlayer.Play();
        }
        else
        {
            playIntro = StartCoroutine(PlayIntro(currentTheme.main, savedTime));
        }
    }

    public void PlayVictoryMusic()
    {
        PlayMusic(currentTheme.victory);
    }

    private void PlayMainMusic()
    {
        PlayMusic(currentTheme.main);
    }

    private void PlayMusic(Music music, float time = 0)
    {
        if (playIntro != null)
        {
            StopCoroutine(playIntro);
            playIntro = null;
        }
        
        if (musicPlayer.isPlaying) musicPlayer.Stop();
        
        if (music.hasIntro)
        {
            playIntro = StartCoroutine(PlayIntro(music, time));
        }
        else
        {
            musicPlayer.clip = music.loop;
            musicPlayer.time = time;
            musicPlayer.loop = true;
            musicPlayer.Play();
        }
    }
    
    private IEnumerator PlayIntro(Music music, float time)
    {
        if (music.loop.loadState == AudioDataLoadState.Unloaded)
        {
            music.loop.LoadAudioData();
        }
        
        musicPlayer.clip = music.intro;
        musicPlayer.loop = false;
        musicPlayer.time = time;
        musicPlayer.Play();
        
        yield return new WaitUntil(() => !musicPlayer.isPlaying);
        
        musicPlayer.clip = music.loop;
        musicPlayer.loop = true;
        musicPlayer.Play();
        
        playIntro = null;
    }
    
    private void ChangeTheme()
    {
        MusicTheme newTheme;
        
        if (Settings.GameMode is GameMode.Single)
        {
            newTheme = PlayersSettings.Player1.Character is PlayerCharacter.Celestia ? celestiaTheme : lunaTheme;
        }
        else
        {
            newTheme = coopTheme;
        }
        
        if (currentTheme == newTheme) return;

        currentTheme = newTheme;
        PlayMusic(currentTheme.main);
    }
}
