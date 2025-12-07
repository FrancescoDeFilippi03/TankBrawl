using Unity.Netcode;
using UnityEngine;

public class NetworkLifecycle : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
    }
}