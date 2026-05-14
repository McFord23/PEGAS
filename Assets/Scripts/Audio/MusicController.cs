using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private AudioSource flyingMusic;
    private AudioSource pauseMusic;
    private AudioSource menuMusic;
    private AudioSource victoryMusic;
    private AudioSource creditsMusic;

    AudioSource celestiaMenuStart;
    AudioSource celestiaMenuLoop;
    AudioSource lunaMenu;

    AudioSource lunaFlying;
    AudioSource celestiaFlyingStart;
    AudioSource celestiaFlyingLoop;

    AudioSource lunaPause;
    AudioSource celestiaPause;

    string scene;

    void Awake()
    {
        scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Main Menu":
                celestiaMenuStart = transform.Find("Celestia Menu (start)").GetComponentInChildren<AudioSource>();
                celestiaMenuLoop = transform.Find("Celestia Menu (loop)").GetComponentInChildren<AudioSource>();
                lunaMenu = transform.Find("Luna Menu").GetComponentInChildren<AudioSource>();

                switch (Global.players[0].character)
                {
                    case Character.Luna:
                        menuMusic = lunaMenu;
                        break;

                    case Character.Celestia:
                        menuMusic = celestiaMenuStart;
                        break;
                }

                menuMusic.Play();
                break;

            case "Game":
                lunaFlying = transform.Find("Luna Flying").GetComponentInChildren<AudioSource>();
                lunaPause = transform.Find("Luna Pause").GetComponentInChildren<AudioSource>();
                celestiaFlyingStart = transform.Find("Celestia Flying (start)").GetComponentInChildren<AudioSource>();
                celestiaFlyingLoop = transform.Find("Celestia Flying (loop)").GetComponentInChildren<AudioSource>();
                celestiaPause = transform.Find("Celestia Pause").GetComponentInChildren<AudioSource>();
                victoryMusic = transform.Find("Victory").GetComponentInChildren<AudioSource>();

                switch (Global.players[0].character)
                {
                    case Character.Luna:
                        flyingMusic = lunaFlying;
                        pauseMusic = lunaPause;
                        break;

                    case Character.Celestia:
                        flyingMusic = celestiaFlyingStart;
                        pauseMusic = celestiaPause;
                        break;
                }

                flyingMusic.Play();
                break;

            case "Credits":
                creditsMusic.Play();
                break;
        }
    }

    private void Update()
    {
        switch (scene)
        {
            case "Main Menu":
                if (Global.players[0].character == Character.Celestia && !menuMusic.isPlaying)
                {
                    menuMusic.Stop();
                    menuMusic = celestiaMenuLoop;
                    menuMusic.Play();
                }
                break;

            case "Game":
                if (Global.players[0].character == Character.Celestia && !(flyingMusic.loop) && (flyingMusic.time >= 116.5f))
                {
                    flyingMusic.Stop();
                    flyingMusic = celestiaFlyingLoop;
                    flyingMusic.Play();
                }
                break;
        }
    }

    public void ChangeTheme(Character theme)
    {
        flyingMusic.Stop();
        
        switch (theme)
        {
            case Character.Luna:
                flyingMusic = lunaFlying;
                pauseMusic = lunaPause;
                break;

            case Character.Celestia:
                flyingMusic = celestiaFlyingStart;
                pauseMusic = celestiaPause;
                break;
        }

        flyingMusic.Play();
    }

    public void PauseFlyMusic()
    {
        flyingMusic.Pause();
        pauseMusic.Play();
    }

    public void ResumeFlyMusic()
    {
        pauseMusic.Stop();
        flyingMusic.UnPause();
    }

    public void PlayVictoryMusic()
    {
        flyingMusic.Stop();
        victoryMusic.Play();
    }
}
