using Unity.Netcode;
using UnityEngine;

public class TankHealthManager : NetworkBehaviour , IDamageble
{
    public NetworkVariable<float> healthNetwork = new NetworkVariable<float>(
        100f ,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<float> shieldNetwork = new NetworkVariable<float>(
        50f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private bool invulnerable = false;
    public bool Invulnerable
    {
        get => invulnerable;
        set => invulnerable = value;
    }

    private float MaxHealth;
    private float MaxShield;


    public override void OnNetworkSpawn()
    {
        healthNetwork.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        healthNetwork.OnValueChanged -= OnHealthChanged;
    }


    private void OnHealthChanged(float previousValue, float newValue)
    {
        if (newValue <= 0 && previousValue > 0)
        {
            Debug.Log("Tank has died.");

            if(IsOwner)
            {
                var tankStateManager = GetComponent<TankStateManager>();
                if(tankStateManager != null)
                {
                    tankStateManager.playerState.Value = TankStateManager.PlayerState.Dead;
                    tankStateManager.CurrentState.ChangeState(tankStateManager.StateFactory.Dead());
                }
            }
        }
    }


    public void InitializeHealth(float health, float shield)
    {
        MaxHealth = health;
        MaxShield = shield;

        if(!IsServer) return;
        healthNetwork.Value = MaxHealth;
        shieldNetwork.Value = MaxShield;
    }

    public void TakeDamage(float damageAmount)
    {
       if(!IsServer || invulnerable) return;

        if (shieldNetwork.Value > 0)
        {
            float shieldDamage = Mathf.Min(shieldNetwork.Value, damageAmount);
            shieldNetwork.Value -= shieldDamage;
            damageAmount -= shieldDamage;
        }
        if (damageAmount > 0)
        {
            healthNetwork.Value = Mathf.Max(healthNetwork.Value - damageAmount, 0);
        }
    }

    public void ResetHealth()
    {
        if(!IsServer) return;
        healthNetwork.Value = MaxHealth;
        shieldNetwork.Value = MaxShield;
    }

    public void Heal(float healAmount)
    {
        if(!IsServer) return;
        healthNetwork.Value = Mathf.Min(healthNetwork.Value + healAmount, MaxHealth);
    }

    public void RechargeShield(float shieldAmount)
    {
        if(!IsServer) return;
        shieldNetwork.Value = Mathf.Min(shieldNetwork.Value + shieldAmount, MaxShield);
    }

}
