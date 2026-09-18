using System.Collections;
using UnityEngine;

public class RoomCamera : CameraBase
{
    [Header("Room")]
    [SerializeField] private Transform gameViewPoint;
    
    private Coroutine move;

    public override void GameMode()
    {
        base.GameMode();
        if (move != null) StopCoroutine(move);
        move = StartCoroutine( Move( gameViewPoint.position ));
    }

    public override void MenuMode()
    {
        base.MenuMode();
        if (move != null) StopCoroutine(move);
        move = StartCoroutine( Move( playersManager.GetPosition() + menuOffset ));
    }

    private IEnumerator Move(Vector3 targetPosition)
    {
        var startPosition = transform.position;
        float progress = 0;
        
        while (progress < 1)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            progress += moveSpeed;
            
            yield return new WaitForFixedUpdate();
        }

        move = null;
    }
}