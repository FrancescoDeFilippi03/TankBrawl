using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class TankPlayerController : NetworkBehaviour
{
    [SerializeField] private TankPlayerData tankPlayerData;
    public TankPlayerData TankPlayerData => tankPlayerData;

    [SerializeField] private TankHealthManager tankHealthManager;
    public TankHealthManager TankHealthManager => tankHealthManager;

    private TankConfigData tankConfigData;
    public TankConfigData TankConfigData => tankConfigData;


    [SerializeField] private TankMovementManager tankMovementManager;
    public TankMovementManager TankMovementManager => tankMovementManager;

    [SerializeField] private TankShootingManager tankShootingManager;
    public TankShootingManager TankShootingManager => tankShootingManager;

    private TankInput tankInput;
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    
    //movement variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    private Vector2 aimInput;
    public Vector2 AimInput => aimInput;


    // Shooting state
    private bool isTriggerHeld = false;

    
    private bool isRedTeam = false;

    //animation variables
    [SerializeField]private Animator tankAnimator;
    public Animator TankAnimator => tankAnimator;


    public override void OnNetworkSpawn()
    {
        tankPlayerData.InitializeTankPlayerData();

        if (!IsOwner) return;
        
        tankInput = new TankInput();
        tankInput.Enable();
        tankInput.Tank.Shoot.performed += OnShootPerformed;
        tankInput.Tank.Shoot.canceled += OnShootCanceled;


        var cameraInScene = FindAnyObjectByType<Unity.Cinemachine.CinemachineCamera>();
        cameraInScene.Target.TrackingTarget = this.transform;
        
        // Rotate camera based on team
        tankConfigData = TeamManager.Instance.GetTankConfigDataForClient(OwnerClientId);
        isRedTeam = tankConfigData.Team == TeamColor.Red;

        if (tankConfigData.Team == TeamColor.Red)
        {
            // Red team spawns facing down, rotate camera 180 degrees
            cameraInScene.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            // Blue team faces up, default rotation
            cameraInScene.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        tankInput.Tank.Shoot.performed -= OnShootPerformed;
        tankInput.Tank.Shoot.canceled -= OnShootCanceled;
        tankInput.Disable();

        tankShootingManager.CursorReset();
    }

    void Update()
    {
        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
        
        // Invert input for red team to match rotated camera
        if (isRedTeam)
        {
            movementInput = -movementInput;
        }
        
        aimInput = tankInput.Tank.Aim.ReadValue<Vector2>();

        TankShootingManager.HandleTurretRotation(aimInput);
        TankShootingManager.Shoot(aimInput, isTriggerHeld);     

    }

    private void OnShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isTriggerHeld = true;
    }
    
    private void OnShootCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isTriggerHeld = false;
    }
    

    public void SetInputActive(bool isActive)
    {
        if (!IsOwner) return;
        if (isActive)
        {
            tankInput.Enable();
        }
        else
        {
            tankInput.Disable();
        }
    }

    public void ResetPlayer()
    {
        isTriggerHeld = false;
        
        TankShootingManager.ShootingSystem.ResetShootingState();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        /* smoothedMovementInput = Vector2.zero;
        currentVelocity = Vector2.zero;
        
        if (weaponPivotTransform != null)
        {
            weaponPivotTransform.rotation = Quaternion.Euler(0, 0, 0);
        } */
    }
}
