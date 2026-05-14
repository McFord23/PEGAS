using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D item)
    {
        if (item.gameObject.CompareTag("Item"))
        {
            EventAdapter.Instance.Execute(EventKey.Victory);
        }
    }
}
