using UnityEngine;
using Unity.Netcode;

public class Tank : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> tankId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    [SerializeField] private TankHealthManager tankHealthManager;
    [SerializeField] private TankMovementManager tankMovementManager;
    [SerializeField] private TankShootingManager tankShootingManager;

    public int TankId => tankId.Value;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        tankId.OnValueChanged += OnTankIdChanged;   
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        tankId.OnValueChanged -= OnTankIdChanged;   
    }

    private void OnTankIdChanged(int previousValue, int newValue)
    {
        Debug.Log($"Tank ID changed to: {newValue}");

        
    }
    public void SetTankId(int id)
    {
        if (!NetworkObject.IsSpawned) {
            return;
        }
        if (IsOwner) tankId.Value = id;
    }

    public void InitializeTankData(TankConfig configData)
    {
        tankHealthManager.InitializeHealth(configData.maxHealth, configData.maxShield);
        tankMovementManager.InitializeMovement(configData.moveSpeed, configData.dashSpeed , configData.dashDuration);
        tankShootingManager.InitializeShooting( 
                                                configData.shootingType, 
                                                configData.fireRate,
                                                configData.fireRange,
                                                configData.damage,
                                                configData.bulletPrefab,
                                                configData.ammoCapacity
                                            );
    }


    public void MoveTank(Vector2 movementInput,Rigidbody2D rb)
    {
        tankMovementManager.MoveTank(movementInput, rb);
    }

    public void HandleRotation(Vector2 inputDirection , Rigidbody2D rb)
    {
        tankMovementManager.HandleRotation(inputDirection, rb);
    }

    public void Shoot(bool isHoldingTrigger)
    {
        tankShootingManager.Shoot(isHoldingTrigger);
    }

    public void Dash(Vector2 dashDirection , Rigidbody2D rb)
    {
        tankMovementManager.Dash(dashDirection, rb);
    }
}
