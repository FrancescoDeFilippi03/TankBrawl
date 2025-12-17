using Unity.Netcode;
using UnityEngine;

public abstract class Distructable : NetworkBehaviour , IDamageble
{
    protected NetworkVariable<float> currentHealth = new NetworkVariable<float>(0f, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected void SetHealthMax(float maxHealthValue)
    {
        currentHealth.Value = maxHealthValue;
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            OnDestroyed();
        }
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Distructable despawned.");
    }
    protected abstract void OnDestroyed();
}
