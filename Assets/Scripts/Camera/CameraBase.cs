using System.Collections;
using UnityEngine;

public class CameraBase : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] protected Vector3 menuOffset = new (2.75f, 0, 0);
    [SerializeField] protected float moveSpeed = 0.1f;
    
    [Header("FOV")]
    [SerializeField] private float fovSpeed = 0.01f;
    [SerializeField] private float gameFOV = 60f;
    [SerializeField] private float menuFOV = 30f;
    
    [Header("Player")]
    [SerializeField] protected PlayersManager playersManager;
    
    protected enum Mode
    {
        Game,
        Menu
    }
    
    protected Mode mode;
    
    private Camera camera;
    private Coroutine setFOV;

    protected virtual void Start()
    {
        camera = GetComponent<Camera>();
    }

    public virtual void GameMode()
    {
        mode = Mode.Game;
        if (setFOV != null) StopCoroutine(setFOV);
        setFOV = StartCoroutine(SetFOV(gameFOV));
    }

    public virtual void MenuMode()
    {
        mode = Mode.Menu;
        if (setFOV != null) StopCoroutine(setFOV);
        setFOV = StartCoroutine(SetFOV(menuFOV));
    }
    
    private IEnumerator SetFOV(float targetFOV)
    {
        var startFOV = camera.fieldOfView;
        float progress = 0;
        
        while (progress < 1)
        {
            camera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, progress);
            progress += fovSpeed;
            
            yield return null;
        }

        setFOV = null;
    }
}