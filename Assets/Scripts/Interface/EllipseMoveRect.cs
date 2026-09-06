using UnityEngine;

public class EllipseMoveRect : MonoBehaviour
{
    private const float RADIAN = 2 * Mathf.PI;
    
    [SerializeField] private float speed = 6;
    [SerializeField] private Vector2 radius = new(2,2);
    private RectTransform rect;
    private Vector2 center;
    private float angle;

    private void Start()
    {
        rect = transform as RectTransform;

        if (rect == null) return;
        
        center = rect.anchoredPosition;
    }

    private void Update()
    {
        angle += speed * Time.deltaTime;
        
        if (angle > RADIAN)
        {
            angle -= RADIAN;
        }
        
        var x = center.x + (radius.x * Mathf.Cos(angle));
        var y = center.y + (radius.y * Mathf.Sin(angle));
        rect.anchoredPosition = new Vector2(x, y);
    }
}