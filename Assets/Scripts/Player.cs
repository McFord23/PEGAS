using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    GameObject pauseMenu;
    GameObject deadMenu;
    public new CameraFollow camera;
    public AudioSource clickSound;

    public Rigidbody2D rb;
    Animator animatorController;
    public MoveState moveState = MoveState.FreeFall;

    // Flap properties
    public float flapForce = 7f;
    float flapTime = 0;
    float flapCooldown = 0.375f;
    public float speed = 0;
    public float acceleration;
    

    // Direction propeties
    public Vector2 spawnPos;
    
    Vector2 mousePos;
    Vector2 pegasPos;
    Vector2 flapDirection;

    Vector2 saveDirection;
    MoveState saveState;

    // Dev ops
    public bool godnessMode = false;
    public bool deathIndicator = false;
    public bool pullClick = false;

    public enum MoveState
    {
        Start,
        FreeFall,
        Hover,
        Flap,
        SlowFall,
        SlowHover,
        SlowFlap,
        Dead,
        Paused
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animatorController = GetComponentInChildren<Animator>();

        pauseMenu = GetComponent<PlayerKeyboardController>().pauseMenu;
        deadMenu = GetComponent<PlayerKeyboardController>().deadMenu;

        spawnPos = transform.position;
    }

    void FixedUpdate()
    {
        if (moveState == MoveState.Flap)
        {
            flapTime -= Time.deltaTime;
            if (flapTime <= 0)
            {
                FreeFall();
            }
        }

        if (moveState == MoveState.Flap || moveState == MoveState.Hover || moveState == MoveState.FreeFall)
        {
            camera.FocusOnFly();
            CalculateDirection();
        }

        acceleration = (rb.velocity.magnitude - speed) / Time.deltaTime;
        
        if (moveState == MoveState.FreeFall)
        {
            rb.AddForce(flapDirection * rb.velocity.magnitude, ForceMode2D.Force);
        }

        speed = rb.velocity.magnitude;
    }

    public void Reset()
    {
        rb.velocity = new Vector2(0, 0);
        transform.position = spawnPos;
        FreeFall();

        deadMenu.SetActive(false);
        camera.FocusOnFly();
    }

    public void Pause()
    {
        saveState = moveState;
        saveDirection = rb.velocity;

        moveState = MoveState.Paused;
        animatorController.Play("FreeFall");

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(0, 0);
    }

    public void Resume()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 1f;
        moveState = saveState;
        rb.AddForce(saveDirection, ForceMode2D.Impulse);
    }

    public void FreeFall()
    {
        moveState = MoveState.FreeFall;
        animatorController.Play("FreeFall");
    }

    public void Flap()
    {
        moveState = MoveState.Flap;
        flapTime = flapCooldown;
        animatorController.Play("Flap");
        rb.AddForce(flapDirection * flapForce, ForceMode2D.Impulse);
    }

    public void Hover()
    {
        moveState = MoveState.Hover;
        animatorController.Play("Hover");
        rb.AddForce(flapDirection, ForceMode2D.Force);
        rb.gravityScale = 0f;
    }

    void CalculateDirection()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pegasPos = transform.position;

        flapDirection = mousePos - pegasPos;
        flapDirection.Normalize();

        if (moveState == MoveState.Hover)
        {
            transform.right = new Vector3(1, 0);
        }
        else
        {
            transform.right = flapDirection;
        }
    }

    void Dead()
    {
        if (!godnessMode)
        {
            moveState = MoveState.Dead;
            animatorController.Play("Dead");

            if (pauseMenu.activeSelf) pauseMenu.SetActive(false);
            clickSound.Play();
            deadMenu.SetActive(true);
            camera.FocusOnPlayer();
        }
    }

    void OnCollisionEnter2D()
    {
        if (!godnessMode) Dead();

        // DevOps.Pull()
        deathIndicator = true;
    }

    // DevOps.Pull()
    private void OnCollisionExit2D()
    {
        deathIndicator = false;
    }

    void OnMouseDown()
    {
        pullClick = true;
    }

    void OnMouseUp()
    {
        pullClick = false;
    }
}