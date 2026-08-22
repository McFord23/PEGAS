using UnityEngine;

public class SoundController : SingletonMonoBehaviour<SoundController>
{
    private Transform flap;
    private AudioSource flapSound;
    private AudioSource[] flapSounds;

    public AudioSource cannonScratchSound;
    public AudioSource cannonShootSound;

    private Transform hit;
    private AudioSource hitSound;
    private AudioSource[] hitSounds;

    private AudioSource headwindSound;
    private AudioSource turnPageSound;
    
    [SerializeField] private PlayersManager playersManager;

    protected override void Awake()
    {
        base.Awake();

        if (SceneManagerAdapter.GetActiveScene() is Level.SantaSisters)
        {
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
        }

        turnPageSound = transform.Find("Turn Page").gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (SceneManagerAdapter.GetActiveScene() is Level.SantaSisters)
        {
            HeadwindVolume();
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

    private void HeadwindVolume()
    {
        headwindSound.volume = Mathf.Pow(playersManager.GetSpeed(), 2) / 4000f;
    }

    public void PlayTurnPageSound()
    {
        turnPageSound.Play();
    }
}
