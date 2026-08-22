using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource flyingMusic;
    private AudioSource pauseMusic;
    private AudioSource menuMusic;
    private AudioSource victoryMusic;
    private AudioSource creditsMusic;

    private AudioSource celestiaMenuStart;
    private AudioSource celestiaMenuLoop;
    private AudioSource lunaMenu;

    private AudioSource lunaFlying;
    private AudioSource celestiaFlyingStart;
    private AudioSource celestiaFlyingLoop;

    private AudioSource lunaPause;
    private AudioSource celestiaPause;

    private Mode mode;

    private enum Mode
    {
        MainMenu,
        Game,
        Credits
    }

    private void Start()
    {
        mode = SceneManagerAdapter.GetActiveScene() switch
        {
            Level.MainMenu => Mode.MainMenu,
            Level.Credits => Mode.Credits,
            _ => Mode.Game
        };

        switch (mode)
        {
            case Mode.MainMenu:
                celestiaMenuStart = transform.Find("Celestia Menu (start)").GetComponentInChildren<AudioSource>();
                celestiaMenuLoop = transform.Find("Celestia Menu (loop)").GetComponentInChildren<AudioSource>();
                lunaMenu = transform.Find("Luna Menu").GetComponentInChildren<AudioSource>();

                switch (PlayersSettings.Player1.Character)
                {
                    case PlayerCharacter.Luna:
                        menuMusic = lunaMenu;
                        break;

                    case PlayerCharacter.Celestia:
                        menuMusic = celestiaMenuStart;
                        break;
                }

                menuMusic.Play();
                break;

            case Mode.Game:
                lunaFlying = transform.Find("Luna Flying").GetComponentInChildren<AudioSource>();
                lunaPause = transform.Find("Luna Pause").GetComponentInChildren<AudioSource>();
                celestiaFlyingStart = transform.Find("Celestia Flying (start)").GetComponentInChildren<AudioSource>();
                celestiaFlyingLoop = transform.Find("Celestia Flying (loop)").GetComponentInChildren<AudioSource>();
                celestiaPause = transform.Find("Celestia Pause").GetComponentInChildren<AudioSource>();
                victoryMusic = transform.Find("Victory").GetComponentInChildren<AudioSource>();

                switch (PlayersSettings.Player1.Character)
                {
                    case PlayerCharacter.Luna:
                        flyingMusic = lunaFlying;
                        pauseMusic = lunaPause;
                        break;

                    case PlayerCharacter.Celestia:
                        flyingMusic = celestiaFlyingStart;
                        pauseMusic = celestiaPause;
                        break;
                }

                flyingMusic.Play();
                break;
            
            case Mode.Credits:
                creditsMusic.Play();
                break;
        }
    }

    private void Update()
    {
        switch (mode)
        {
            case Mode.MainMenu:
                if (PlayersSettings.Player1.Character == PlayerCharacter.Celestia && !menuMusic.isPlaying)
                {
                    menuMusic.Stop();
                    menuMusic = celestiaMenuLoop;
                    menuMusic.Play();
                }
                break;

            case Mode.Game:
                if (PlayersSettings.Player1.Character == PlayerCharacter.Celestia && !(flyingMusic.loop) && (flyingMusic.time >= 116.5f))
                {
                    flyingMusic.Stop();
                    flyingMusic = celestiaFlyingLoop;
                    flyingMusic.Play();
                }
                break;
        }
    }

    public void ChangeTheme(PlayerCharacter theme)
    {
        flyingMusic.Stop();
        
        switch (theme)
        {
            case PlayerCharacter.Luna:
                flyingMusic = lunaFlying;
                pauseMusic = lunaPause;
                break;

            case PlayerCharacter.Celestia:
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
