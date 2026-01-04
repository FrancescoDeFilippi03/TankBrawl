using UnityEngine;

public class ShieldCollectible :  Collectable
{
    [SerializeField] private float shieldAmount = 25f;
    public override void OnCollect(TankPlayerController collector)
    {
        if (collector.TryGetComponent<Tank>(out var tank))
        {
            tank.RechargeShield(shieldAmount);
            isSpawned.Value = false;
        }
    }
}
