using UnityEngine;

public class Snowfall : MonoBehaviour
{
    ParticleSystem[] particleSystems;
    ParticleSystem.EmissionModule[] emissions;

    void Start()
    {
        particleSystems = new ParticleSystem[transform.childCount];
        emissions = new ParticleSystem.EmissionModule[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            particleSystems[i] = transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
            emissions[i] = particleSystems[i].emission; 
        }

        Active(true);
    }

    public void Active(bool val)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            emissions[i].enabled = val;
        }
    }

    public void Pause()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            particleSystems[i].Pause();
        }
    }

    public void Resume()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            particleSystems[i].Play();
        }
    }
}
