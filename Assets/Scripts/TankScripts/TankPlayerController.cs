using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class TankPlayerController : NetworkBehaviour
{

    private TankConfigData tankConfigData;
    public TankConfigData TankConfigData => tankConfigData;
    [SerializeField] private Tank tank;
    public Tank Tank => tank;
    private TankInput tankInput;
    
    //movement variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    private Vector2 aimInput;
    public Vector2 AimInput => aimInput;


    // Shooting state
    private bool isTriggerHeld = false;

    public bool isDashing = false;
    
    private bool isRedTeam = false;

    //animation variables
    [SerializeField]private Animator tankAnimator;
    public Animator TankAnimator => tankAnimator;


    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        
        tankInput = new TankInput();
        tankInput.Enable();
        tankInput.Tank.Shoot.performed += OnShootPerformed;
        tankInput.Tank.Shoot.canceled += OnShootCanceled;

        tankInput.Tank.Dash.performed += OnDashPerformed;

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
        tankInput.Tank.Dash.performed -= OnDashPerformed;
        tankInput.Disable();

    }

    void Update()
    {
        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
        if (isRedTeam)
        {
            movementInput = -movementInput;
        }
        aimInput = tankInput.Tank.Aim.ReadValue<Vector2>();
        
        tank.Shoot(isTriggerHeld);
    }

    private void OnShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isTriggerHeld = true;
    }
    
    private void OnShootCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isTriggerHeld = false;
    }
    
    private void OnDashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        
        if (!isDashing)
        {
            isDashing = true;
        }
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
        tank.ResetVelocity();

        /* smoothedMovementInput = Vector2.zero;
        currentVelocity = Vector2.zero;
        
        if (weaponPivotTransform != null)
        {
            weaponPivotTransform.rotation = Quaternion.Euler(0, 0, 0);
        } */
    }
}
