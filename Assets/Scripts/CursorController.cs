using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    private bool isMenuActive;
    private Vector2 oldMousePos;

    private void Start()
    {
        oldMousePos = Mouse.current.position.ReadValue();
    }

    private void Update()
    {
        if (!isMenuActive) return;
        
        if (Controls.Navigation != Vector2.zero)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Mouse.current.position.ReadValue() != oldMousePos && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        oldMousePos = Mouse.current.position.ReadValue();
    }

    public void StartMonitoring()
    {
        isMenuActive = true;
    }

    public void StopMonitoring()
    {
        isMenuActive = false;
    }

    public void CursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CursorUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CursorVisible()
    {
        Cursor.visible = true;
    }

    public void CursorUnvisible()
    {
        Cursor.visible = false;
    }
}
