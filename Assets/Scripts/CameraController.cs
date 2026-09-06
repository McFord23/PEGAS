using UnityEngine;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    private Camera controllableCamera;
    private Rigidbody2D rigidbody;
    private PlayersManager playersManager;

    private Mode mode;

    private enum Mode
    {
        Fly,
        Player
    }

    [SerializeField]  private Vector3 playerOffset = new (10, 0, 0);
    [SerializeField]  private Vector3 interfaceOffset = new (2.75f, 0, 0);
    [SerializeField]  private Vector3 mapOffset = new (0, 0, -10);
    
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 0.04f;

    //[SerializeField] private float maxSize = 90f; // ort = 25
    [SerializeField] private float flySize = 60f; // ort = 13
    [SerializeField] private float minSize = 30f; // ort = 5
    
    private float size;

    private void Start()
    {
        controllableCamera = GetComponent<Camera>();
        playersManager = PlayersManager.Instance;
        rigidbody = GetComponent<Rigidbody2D>();
        size = flySize;
    }

    private void FixedUpdate()
    {
        if (Global.IsLoading) return;
        
        var playerPosition = playersManager.GetPosition();
        if (playerPosition == Vector3.zero)
        {
            return;
        }
        
        var target = transform.position;
        
        // max player speed = 184
        var speed = Mathf.Max(playersManager.GetSpeed() / 70, 1) * moveSpeed;

        switch (mode)
        {
            case Mode.Fly:
                var offset = playersManager.GetDirection() >= 0 ? playerOffset : -playerOffset;
                target = Vector3.Lerp(rigidbody.position, playerPosition + offset + mapOffset, speed);
                size = flySize;
                break;
            
            case Mode.Player:
                target = Vector3.Lerp(rigidbody.position, playerPosition + interfaceOffset + mapOffset, 1.5f * speed);
                size = minSize;
                break;
        }

        rigidbody.MovePosition(target);
        controllableCamera.fieldOfView = Mathf.Lerp(controllableCamera.fieldOfView, size, zoomSpeed);
    }

    public void FocusOnFly()
    {
        mode = Mode.Fly;
    }

    public void FocusOnPlayer()
    {
        mode = Mode.Player;
    }
}