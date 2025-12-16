using UnityEngine;

public class ShieldCollectible :  ICollectable
{
    [SerializeField] private float shieldAmount = 25f;
    public override void OnCollect(TankPlayerController collector)
    {
        if (collector.TryGetComponent<TankHealthManager>(out var tankHealth))
        {
            tankHealth.RechargeShield(shieldAmount);
            isSpawned.Value = false;
        }
    }
}
