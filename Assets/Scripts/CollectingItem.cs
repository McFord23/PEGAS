using UnityEngine;

public class CollectingItem : MonoBehaviour
{
    public Transform owner;

    private Vector3 spawnPos;
    private Quaternion spawnRot;

    private BoxCollider2D trigger;
    private ItemAdapter adapter;

    private void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
        adapter = GetComponent<ItemAdapter>();

        var tempTransform = transform;
        spawnPos = tempTransform.position;
        spawnRot = tempTransform.rotation;
        
        PlayersManager.Instance.ResetEvent.AddListener(ExecuteReset);
    }

    private void LateUpdate()
    {
        if (!owner) return;
        transform.position = owner.position;
    }

    public void ExecuteReset()
    {
        adapter.Reset();
    }
    
    public void Reset()
    {
        owner = null;
        
        trigger.enabled = true;
        
        transform.position = spawnPos;
        transform.rotation = spawnRot;
    }

    public void ExecutePickUp(Transform character)
    {
        adapter.PickUp(character);
    }

    public void Pickup(Transform character)
    {
        if (!owner) owner = character;
    }

    public void ExecuteDrop()
    {
        adapter.Drop();
    }

    public void Drop()
    {
        owner = null;
    }
}
