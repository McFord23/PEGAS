using UnityEngine;
using UnityEngine.Serialization;

public class HeadwindEffect : MonoBehaviour
{
    private PlayersManager playersManager;
    private Transform view;
    
    private float ratio;
    private Vector3 offset;

    private ParticleSystem headwind;
    private ParticleSystem.MainModule main;
    private ParticleSystem.EmissionModule emission;

    private void Start()
    {
        playersManager = PlayersManager.Instance;
        view = Camera.main.transform;
        headwind = GetComponent<ParticleSystem>();

        main = headwind.main;
        emission = headwind.emission;

        offset = transform.position - view.position;
    }

    private void Update()
    {
        var localTranform = transform;
        
        ratio = playersManager.GetSpeed() / 150f;
        main.startSpeed = ratio * 100;
        emission.rateOverTime = ratio * 250;

        float x = view.position.x + playersManager.GetDirection() * offset.x;
        localTranform.position = new Vector3(x, localTranform.position.y, 0);

        Quaternion rot = localTranform.rotation;
        rot.eulerAngles = new Vector3(0, playersManager.GetDirection() * -90, 0);
        localTranform.rotation = rot;
    }
}
