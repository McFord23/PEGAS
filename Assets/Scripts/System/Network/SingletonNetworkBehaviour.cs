using Unity.Netcode;

public class SingletonNetworkBehaviour<T>: NetworkBehaviour where T : SingletonNetworkBehaviour<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        
        Instance = this as T;
    }
}
