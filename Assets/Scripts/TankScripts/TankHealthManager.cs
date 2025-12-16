using Unity.Netcode;
using UnityEngine;

public class TankHealthManager : NetworkBehaviour
{
    private float health;
    public float Health => health;
    private float shield;
    public float Shield => shield;

    private bool invulnerable = false;
    public bool Invulnerable
    {
        get => invulnerable;
        set => invulnerable = value;
    }

    public void InitializeHealth(TankPlayerData tankPlayerData)
    {
        health = tankPlayerData.TankBase.health;
        shield = tankPlayerData.TankBase.armor;
    }

    public void TakeDamage(float damageAmount)
    {
        float damageAfterShield = damageAmount - shield;
        if (damageAfterShield > 0)
        {
            shield = 0;
            health -= damageAfterShield;
        }
        else
        {
            shield -= damageAmount;
        }

    }


}
