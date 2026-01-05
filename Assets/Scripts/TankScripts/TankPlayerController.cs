using UnityEngine;
using Unity.Netcode;
public class TankPlayerController : NetworkBehaviour
{
    [SerializeField] private Tank tank;
    public Tank Tank => tank;
    private TankInput tankInput;
    
    //movement input variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;
    private Vector2 aimInput;
    public Vector2 AimInput => aimInput;


    //shooting input variables
    private bool isTriggerHeld = false;


    //dashing input variables
    private bool isDashing = false;


    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        
        tankInput = new TankInput();
        tankInput.Enable();

        tankInput.Tank.Shoot.performed += OnShootPerformed;
        tankInput.Tank.Shoot.canceled += OnShootCanceled;
        tankInput.Tank.Dash.performed += OnDashPerformed;

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
        aimInput = tankInput.Tank.Aim.ReadValue<Vector2>();
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
        
        isDashing = true;
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
    }
}
