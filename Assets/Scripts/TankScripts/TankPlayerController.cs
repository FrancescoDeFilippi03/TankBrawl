using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class TankPlayerController : NetworkBehaviour
{
    [SerializeField] private TankPlayerData tankPlayerData;
    public TankPlayerData TankPlayerData => tankPlayerData;

    [SerializeField] private TankVisuals tankVisuals;
    public TankVisuals TankVisuals => tankVisuals;

    [SerializeField] private TankHealthManager tankHealthManager;
    public TankHealthManager TankHealthManager => tankHealthManager;

    private TankInput tankInput;
    [SerializeField] private Rigidbody2D rb;
    
    //movement variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    private Vector2 smoothedMovementInput;
    public Vector2 SmoothedMovementInput
    {
        get => smoothedMovementInput;
        set => smoothedMovementInput = value;
    }

    private Vector2 aimInput;
    public Vector2 AimInput => aimInput;

    [SerializeField] private float movementSmoothing = 5f;
    public float MovementSmoothing => movementSmoothing;

    [SerializeField] private float rotationSmoothing = 7f;
    public float RotationSmoothing => rotationSmoothing;

    Vector2 aimDirection;
    public Vector2 AimDirection => aimDirection;
    private Vector2 currentVelocity = Vector2.zero;
    
    // Shooting state
    private bool isTriggerHeld = false;

    [SerializeField] private Transform weaponPivotTransform;
    public Transform WeaponPivotTransform => weaponPivotTransform;

    [SerializeField] ShootingSystem shootingSystem;


    //animation variables
    [SerializeField]private Animator tankAnimator;
    public Animator TankAnimator => tankAnimator;


    public override void OnNetworkSpawn()
    {

        //eseguo qui per owner e non owner
        TankConfigData tankConfigData = TeamManager.Instance.GetTankConfigDataForClient(OwnerClientId);
        tankPlayerData.InitTankElements(
            tankConfigData
        );

        tankVisuals.InitializeVisuals(tankConfigData);

        shootingSystem.InitWeapon(
            tankPlayerData.TankWeapon,
            tankPlayerData.TankWeapon.ammo,
            tankVisuals.WeaponInstance.GetComponent<WeaponFirePoints>().firePoints
        );

        tankHealthManager.InitializeHealth(
            tankPlayerData
        );

        if (!IsOwner) return;
        
        tankInput = new TankInput();
        tankInput.Enable();
        tankInput.Tank.Shoot.performed += OnShootPerformed;
        tankInput.Tank.Shoot.canceled += OnShootCanceled;
        
        CursorInitialization();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        tankInput.Tank.Shoot.performed -= OnShootPerformed;
        tankInput.Tank.Shoot.canceled -= OnShootCanceled;
        tankInput.Disable();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
        aimInput = tankInput.Tank.Aim.ReadValue<Vector2>();

        HandleTurretRotation();
        HandleShooting();
    }

    Vector2 GetAimDirection(){
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 0f));
        Vector2 direction = (worldMousePosition - weaponPivotTransform.position).normalized;
        return direction;
    }

    void HandleTurretRotation()
    {
        
        aimDirection = GetAimDirection();

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        weaponPivotTransform.rotation = Quaternion.Slerp(
            weaponPivotTransform.rotation,
            targetRotation,
            Time.fixedDeltaTime * rotationSmoothing
        );
    }

    private void OnShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isTriggerHeld = true;
    }
    
    private void OnShootCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isTriggerHeld = false;
    }
    
    private void HandleShooting()
    {
        shootingSystem.TryShoot(aimDirection, isTriggerHeld);
    }


    public void MoveTank()
    {
       SmoothedMovementInput = Vector2.SmoothDamp(
            SmoothedMovementInput,
            MovementInput,
            ref currentVelocity,
            1f / movementSmoothing
        );

        if (SmoothedMovementInput.magnitude > 0.01f)
        {
            Vector2 movement = tankPlayerData.TankBase.speed * Time.fixedDeltaTime * SmoothedMovementInput;
            rb.MovePosition(rb.position + movement);
        }

        if (MovementInput.magnitude > 0.1f)
        {
            HandleRotation(MovementInput);
        }
    }

    private void HandleRotation(Vector2 inputDirection)
    {
        float targetAngle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        Quaternion currentRotation = Quaternion.Euler(0, 0, rb.rotation);

        Quaternion newRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            RotationSmoothing * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation.eulerAngles.z);
    }


    void CursorInitialization()
    {
        Sprite crosshairSprite = tankPlayerData.TankWeapon.crosshairSprite;
        if (crosshairSprite != null)
        {
            Texture2D cursorTexture = crosshairSprite.texture;
            Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
        Cursor.visible = true;
    }
}
