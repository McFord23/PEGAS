using Unity.Netcode;
using UnityEngine;

/**
 * Не дает кривому нетворку юнити спавниться второй раз, если вернуться обратно в меню
 */
public class NetworkManagerDuplicateFix : MonoBehaviour
{
    private void Awake()
    {
        var managersCount = FindObjectsByType<NetworkManager>().Length;
        if (managersCount > 1)
        {
            Destroy(gameObject);
        }
    }
}
