using UnityEngine;
using Unity.Netcode;
public class BarrelDistructable : Distructable
{
    public float maxHealth = 50f;

    [SerializeField] private NetworkObject explosionEffectPrefab;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SetHealthMax(maxHealth);
        }
    }


    protected override void OnDestroyed()
    {
        if (explosionEffectPrefab != null)
        {
            NetworkObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            explosionEffect.Spawn();
        }

        NetworkObject.Despawn();
    }
}
