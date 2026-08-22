using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Сredits : MonoBehaviour
{
    [SerializeField] private float creditsSpeed = 0.85f;
    [SerializeField] private float skipSpeed = 0.01f;
    [SerializeField] private Text skip;

    private AudioSource music;
    
    private void Start()
    {
        skip.color = new Color(skip.color.r, skip.color.g, skip.color.b,0);
        skip.gameObject.SetActive(false);

        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!music.isPlaying)
        {
            Exit();
        }
        
        if (Controls.Apply || Controls.Pause || Mouse.current.leftButton.isPressed)
        {
            if (skip.color.a == 0)
            {
                skip.gameObject.SetActive(true);
                skip.color = new Color(skip.color.r, skip.color.g, skip.color.b,1);
                return;
            }
        }

        if (Controls.Apply || Controls.Pause)
        {
            if (skip.color.a != 0)
            {
                Exit();
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(0f, creditsSpeed * Time.deltaTime, 0f);
        
        if (skip.color.a == 0) return;
        
        var albedo = Mathf.Clamp(skip.color.a - skipSpeed, 0, 1);
        skip.color = new Color(skip.color.r, skip.color.g, skip.color.b,albedo);
        
        if (skip.color.a == 0) skip.gameObject.SetActive(false);
    }

    public void Exit()
    {
        SceneManagerAdapter.Instance.LoadScene(Level.MainMenu);
    }
}
