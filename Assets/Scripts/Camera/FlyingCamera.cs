using UnityEngine;

public class FlyingCamera : CameraBase
{
    [Header("Flying")]
    [SerializeField] private Vector3 flyingOffset = new (10, 0, 0);
    [SerializeField] private Vector3 playerOffset = new (0, 0, -10);
    
    private Rigidbody2D rigidbody;

    protected override void Start()
    {
        base.Start();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Global.IsLoading) return;
        
        // max player speed = 184
        var speedRatio = playersManager.GetSpeed() / 70;
        var speed = Mathf.Max(speedRatio, 1) * moveSpeed;
        var target = playerOffset;

        switch (mode)
        {
            case Mode.Game:
                target += playersManager.GetPosition();
                var offset = playersManager.GetDirection() >= 0 ? flyingOffset : -flyingOffset;
                target = Vector3.Lerp(rigidbody.position, target + (offset * speedRatio), speed);
                break;
            
            case Mode.Menu:
                target += playersManager.GetPositionForMenu();
                target = Vector3.Lerp(rigidbody.position, target + (speedRatio * Vector3.right) + menuOffset, speed);
                break;
        }

        rigidbody.MovePosition(target);
    }
}