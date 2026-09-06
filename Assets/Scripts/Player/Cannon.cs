using UnityEngine;
using UnityEngine.Events;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float power = 100f;
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    [SerializeField] private Vector3 direction;
    [SerializeField] private AudioSource scratchAudio;
    [SerializeField] private AudioSource shootAudio;

    private PlayerBase player;
    private Rigidbody2D rb;
    private bool active;

    public UnityEvent CannonShootEvent;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!active) return;
        if (Global.IsPause) return;

        var powerInput = player.MoveInput.x;
        var rotateInput = player.MoveInput.y;
        var shootInput = player.MainActionInput;
        
        if ((360 - transform.eulerAngles.z) > 300)
        {
            if (transform.eulerAngles.z < 55)
            {
                rb.AddTorque(rotateInput);
            }
            else if (rotateInput < 0)
            {
                rb.AddTorque(rotateInput);
            }
        }
        else
        {
            if (rotateInput > 0)
            {
                rb.AddTorque(rotateInput);
            }
        }

        direction = transform.right;
        direction.Normalize();

        scratchAudio.volume = Mathf.Clamp(Mathf.Abs(rb.angularVelocity / 20), 0f, 0.5f);
        

        power = Mathf.Clamp(power + powerInput, 20000, 40000);

        if (shootInput > 0)
        {
            Shoot();
        }
            
        angle = transform.eulerAngles.z;
    }
    
    public void OnReset()
    {
        if (active)
        {
            active = false;
            scratchAudio.Stop();
        }
    }

    public void Pause()
    {
        if (active) scratchAudio.Pause();
    }

    public void Resume()
    {
        if (active) scratchAudio.UnPause();
    }

    public void Interact(PlayerBase playerBase)
    {
        active = true;
        player = playerBase;
        scratchAudio.Play();
    }

    public void Shoot()
    {
        active = false;
        CannonShootEvent.Invoke();
        shootAudio.Play();
        scratchAudio.Stop();  
    }
}
