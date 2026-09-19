using System.Collections;
using UnityEngine;

public class RoomCamera : CameraBase
{
    [Header("Room")]
    [SerializeField] private Transform gameViewPoint;
    [SerializeField] private Vector3 gameRotation = new (60, 0, 0);
    [SerializeField] private Vector3 menuRotation = new (30, 0, 0);
    
    private Coroutine move;
    private Coroutine rotate;

    public override void GameMode()
    {
        base.GameMode();
        
        if (move != null) StopCoroutine(move);
        move = StartCoroutine( Move( gameViewPoint.position ));
        
        if (rotate != null) StopCoroutine(rotate);
        rotate = StartCoroutine( Rotate( gameRotation ));
    }

    public override void MenuMode()
    {
        base.MenuMode();
        
        if (move != null) StopCoroutine(move);
        move = StartCoroutine( Move( playersManager.GetPositionForMenu() + menuOffset ));
        
        if (rotate != null) StopCoroutine(rotate);
        rotate = StartCoroutine( Rotate( menuRotation ));
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

        transform.position = targetPosition;
        move = null;
    }
    
    private IEnumerator Rotate(Vector3 target)
    {
        var startRotation = transform.rotation;
        var targetRotation = Quaternion.Euler(target);
        float progress = 0;
        
        while (progress < 1)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
            progress += moveSpeed;
            
            yield return new WaitForFixedUpdate();
        }

        transform.rotation = targetRotation;
        rotate = null;
    }
}