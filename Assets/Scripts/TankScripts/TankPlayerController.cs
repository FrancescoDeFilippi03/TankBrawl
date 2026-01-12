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
    public bool IsTriggerHeld => isTriggerHeld;

    //dashing input variables
    private bool isDashing = false;
    public bool IsDashing
    {
        get
        {
            if (isDashing)
            {
                isDashing = false;
                return true;
            }
            return false;
        }
    }

    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        
        tankInput = new TankInput();
        
        tankInput.Enable();

        tankInput.Tank.Shoot.performed += _ => isTriggerHeld = true;
        tankInput.Tank.Shoot.canceled += _ => isTriggerHeld = false;
        tankInput.Tank.Dash.performed += _ => isDashing = true;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        tankInput.Disable();
    }

    void Update()
    {
        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
        if(Tank.isRedTeam)
        {
            movementInput = -movementInput;
        }
        aimInput = tankInput.Tank.Aim.ReadValue<Vector2>();

        Tank.Shoot(IsTriggerHeld);
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
