using Unity.Netcode;
using UnityEngine;

public class HealthCollectable : Collectable
{
    [SerializeField] private float healthAmount = 25f;
    public override void OnCollect(TankPlayerController collector)
    {
        if (collector.TryGetComponent<Tank>(out var tank))
        {
            tank.Heal(healthAmount);
            isSpawned.Value = false;
        }
    }

}
