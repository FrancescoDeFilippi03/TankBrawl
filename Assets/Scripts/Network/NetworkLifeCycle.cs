using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkLifecycle : MonoBehaviour
{
    public static Dictionary<ulong, string> ClientIdToPlayerId = new Dictionary<ulong, string>();
    private void Awake()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
    }

}