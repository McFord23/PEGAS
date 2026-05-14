using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SoundController : SingletonMonoBehaviour<SoundController>
{
    Transform flap;
    AudioSource flapSound;
    AudioSource[] flapSounds;

    public AudioSource cannonScratchSound;
    public AudioSource cannonShootSound;

    Transform hit;
    AudioSource hitSound;
    AudioSource[] hitSounds;

    AudioSource headwindSound;
    AudioSource turnPageSound;

    [FormerlySerializedAs("playersController")] 
    public PlayersManager playersManager;
    string scene;

    protected override void Awake()
    {
        base.Awake();
        scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Game":
                flap = transform.Find("Flap");
                flapSounds = new AudioSource[flap.transform.childCount];
                for (int i = 0; i < flap.transform.childCount; i++)
                {
                    flapSounds[i] = flap.transform.GetChild(i).gameObject.GetComponent<AudioSource>();
                }

                cannonScratchSound = transform.Find("Cannon Scratch").gameObject.GetComponent<AudioSource>();
                cannonShootSound = transform.Find("Cannon Shoot").gameObject.GetComponent<AudioSource>();

                hit = transform.Find("Hit");
                hitSounds = new AudioSource[hit.transform.childCount];
                for (int k = 0; k < hit.transform.childCount; k++)
                {
                    hitSounds[k] = hit.transform.GetChild(k).gameObject.GetComponent<AudioSource>();
                }

                headwindSound = transform.Find("Headwind").gameObject.GetComponent<AudioSource>();
                break;
        }

        turnPageSound = transform.Find("Turn Page").gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch (scene)
        {
            case "Game":
                HeadwindVolume();
                break;
        }
    }

    public void Flap()
    {
        flapSound = flapSounds[Random.Range(0, flap.transform.childCount)];
        flapSound.Play();
    }

    public void Hit()
    {
        hitSound = hitSounds[Random.Range(0, hit.transform.childCount)];
        hitSound.Play();
    }

    void HeadwindVolume()
    {
        headwindSound.volume = Mathf.Pow(playersManager.GetSpeed(), 2) / 4000f;
    }

    public void PlayTurnPageSound()
    {
        turnPageSound.Play();
    }
}
