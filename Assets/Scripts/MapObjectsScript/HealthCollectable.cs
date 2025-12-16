using Unity.Netcode;
using UnityEngine;

public class HealthCollectable : ICollectable
{
    [SerializeField] private float healthAmount = 25f;
    public override void OnCollect(TankPlayerController collector)
    {
        if (collector.TryGetComponent<TankHealthManager>(out var tankHealth))
        {
            tankHealth.Heal(healthAmount);
            isSpawned.Value = false;
        }
    }

}
